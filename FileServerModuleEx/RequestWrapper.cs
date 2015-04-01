//-----------------------------------------------------------------------
// <copyright file="RequestWrapper.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using Sphorium.WebDAV.Examples.FileServer.Properties;
using Sphorium.WebDAV.Server.Framework.Classes;
using Sphorium.WebDAV.Server.Framework.Collections;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Summary description for RequestWrapper.
	/// </summary>
	public class RequestWrapper
	{
		private static readonly string _localPath = Settings.Default.RepositoryPath;

		/// <summary>
		/// Private constructor
		/// </summary>
		private RequestWrapper() { }

		public static string WebURLRoot
		{
			get
			{
				string _webURLRoot = "";
				if (HttpContext.Current != null)
				{
					HttpRequest _request = HttpContext.Current.Request;
					_webURLRoot = _request.Url.GetLeftPart(UriPartial.Authority) + _request.ApplicationPath;
				}

				return _webURLRoot;
			}
		}

		public static string WebBasePath
		{
			get
			{
				string _basePath = "";
				if (HttpContext.Current != null)
				{
					_basePath = HttpContext.Current.Request.Url.AbsoluteUri;

					if (!Path.HasExtension(_basePath) && !_basePath.EndsWith("/"))
						_basePath += "/";
				}

				return _basePath;
			}
		}

		private static string GetPhysicalPath(string path)
		{
			return GetPhysicalPath(path, 0);
		}

		private static string GetPhysicalPath(string path, int removeEndTokenCount)
		{
			string _relativePath = path;

			if (_relativePath.StartsWith(RequestWrapper.WebURLRoot))
			{
				//Make it relative
				_relativePath = _relativePath.Remove(0, RequestWrapper.WebURLRoot.Length);
			}

			StringBuilder _relativePhysicalPath = new StringBuilder();
			string[] _path = _relativePath.Split('/');

			for (int i = 0; i < _path.Length - removeEndTokenCount; i++)
			{
				string _item = _path[i];
				if (_item.Length > 0)
					_relativePhysicalPath.Append(_item + @"\");
			}

			//Append the localpath and remove the last \
			string _physicalPath;

			if (!_localPath.EndsWith(@"\"))
				_physicalPath = _localPath + @"\" + _relativePhysicalPath;
			else
				_physicalPath = _localPath + _relativePhysicalPath;

			if (_physicalPath.EndsWith(@"\"))
				_physicalPath = _physicalPath.Remove(_physicalPath.Length - 1, 1);

			return _physicalPath;
		}


		/// <summary>
		/// Retrieves the resource name
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns></returns>
		public static string GetResourceName(string urlPath)
		{
			return Path.GetFileName(urlPath);
		}

		/// <summary>
		/// Returns the physical path to a requested resource
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns></returns>
		public static string GetResourcePath(string urlPath)
		{
			return GetPhysicalPath(urlPath);
		}

		/// <summary>
		/// Verifies the requested resource is valid
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns></returns>
		public static bool ValidResourceByPath(string urlPath)
		{
			if (GetDirectory(urlPath) != null)
				return true;
			else if (GetFile(urlPath) != null)
				return true;

			return false;
		}

		/// <summary>
		/// Retrieves a directory
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns>Null if the directory does not exist</returns>
		public static DirectoryInfo GetDirectory(string urlPath)
		{
			string _physicalPath = GetPhysicalPath(urlPath);

			if (Directory.Exists(_physicalPath))
				return new DirectoryInfo(_physicalPath);

			return null;
		}


		/// <summary>
		/// Retrieves a directory / file's parent directory
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns>Null if the parent directory does not exist</returns>
		public static DirectoryInfo GetParentDirectory(string urlPath)
		{
			string _physicalPath = GetPhysicalPath(urlPath);

			DirectoryInfo _dirInfo = new DirectoryInfo(_physicalPath);
			if (_dirInfo.Exists)
				return _dirInfo.Parent;
			else
			{
				//The requested file or folder may not exist... let's see if the parent does
				_physicalPath = GetPhysicalPath(urlPath, 1);

				if (Directory.Exists(_physicalPath))
					return new DirectoryInfo(_physicalPath);
			}

			return null;
		}


		/// <summary>
		/// Retrieves a file
		/// </summary>
		/// <param name="urlPath">Absolute or relative URL path</param>
		/// <returns>Null if the file does not exist</returns>
		public static FileInfo GetFile(string urlPath)
		{
			string _physicalFilePath = GetPhysicalPath(urlPath);

			if (File.Exists(_physicalFilePath))
				return new FileInfo(_physicalFilePath);

			return null;
		}


		/// <summary>
		/// Retrieve's a file's lock information file path (*.locked)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string GetLockInfoFilePath(string filePath)
		{
			string _lockFilePath = null;

			FileInfo _fileInfo = new FileInfo(filePath);
			if (_fileInfo.Exists)
				_lockFilePath = _fileInfo.FullName + "._.locked";

			return _lockFilePath;
		}

		/// <summary>
		/// Retrieve's a file's lock information or loads a locked file (*.locked)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static DavLockProperty GetLockInfo(string filePath)
		{
			DavLockProperty _lockProperty = null;

			//Try to deserialize the lock file
			string _lockFilePath = GetLockInfoFilePath(filePath);
			if (_lockFilePath != null)
			{
				try
				{
					FileInfo _lockFile = new FileInfo(_lockFilePath);
					using (Stream _lockFileStream = _lockFile.Open(FileMode.Open))
					{
						BinaryFormatter _binaryFormatter = new BinaryFormatter();
						_lockProperty = (DavLockProperty)_binaryFormatter.Deserialize(_lockFileStream);
						_lockFileStream.Close();
					}
				}
				catch (Exception)
				{
					//Incase the deserialization fails
				}
			}

			return _lockProperty;
		}


		/// <summary>
		/// Retrieve's a file's latest file version or loads a locked file (*.version)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static FileInfo GetLatestFileVersion(string filePath)
		{
			string _physicalFilePath = GetPhysicalPath(filePath);

			FileInfo _latestVersionFileInfo = null;
			if (Path.GetExtension(filePath) == ".latestVersion")
				_latestVersionFileInfo = new FileInfo(_physicalFilePath);

			else
			{
				//Attempt to retrieve the latest version path
				DirectoryInfo _dirInfo = new DirectoryInfo(Path.GetDirectoryName(_physicalFilePath));
				if (_dirInfo.Exists)
				{
					FileInfo[] _versionFiles = _dirInfo.GetFiles(Path.GetFileName(_physicalFilePath) + "._.latestVersion");

					if (_versionFiles.Length == 1)
						_latestVersionFileInfo = _versionFiles[0];
				}
			}

			return _latestVersionFileInfo;
		}


		/// <summary>
		/// Parses the version's file name
		/// </summary>
		/// <param name="versionFileInfo"></param>
		/// <returns></returns>
		public static string ParseVersionFileName(FileInfo versionFileInfo)
		{
			string _versionFileName = versionFileInfo.Name;

			string _delimiter = "._.";
			int _prefixIndex = _versionFileName.IndexOf(_delimiter);
			if (_prefixIndex != -1)
				_versionFileName = _versionFileName.Substring(0, _prefixIndex);

			return _versionFileName;
		}

		/// <summary>
		///	Retrieve's a file's custom property information file path (*.property)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string GetCustomPropertyInfoFilePath(string filePath)
		{
			string _propFilePath = null;

			FileInfo _fileInfo = new FileInfo(filePath);
			if (_fileInfo.Exists)
				_propFilePath = _fileInfo.FullName + "._.property";

			return _propFilePath;
		}

		/// <summary>
		///	Retrieve's a file's custom property information or loads a propertyfile (*.property)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static DavPropertyCollection GetCustomPropertyInfo(string filePath)
		{
			DavPropertyCollection _customProperties = null;

			//Try to deserialize the property file
			string _propFilePath = GetCustomPropertyInfoFilePath(filePath);
			if (_propFilePath != null)
			{
				try
				{
					FileInfo _propFile = new FileInfo(_propFilePath);
					if (_propFile.Exists)
					{
						using (Stream _propFileStream = _propFile.Open(FileMode.Open))
						{
							BinaryFormatter _binaryFormatter = new BinaryFormatter();
							_customProperties = (DavPropertyCollection)_binaryFormatter.Deserialize(_propFileStream);
							_propFileStream.Close();
						}
					}
				}
				catch (Exception)
				{
					//Incase the deserialization fails
				}
			}

			return _customProperties;
		}

	}
}
