//-----------------------------------------------------------------------
// <copyright file="DavUnlock.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Sphorium.WebDAV.Examples.FileServer.Properties;
using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavUnlock.
	/// </summary>
	public sealed class DavUnlock : DavUnlockBase
	{
		public DavUnlock()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavUnlock_ProcessDavRequest);
		}

		private void DavUnlock_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to see if a lock already exists
			DirectoryInfo _dirInfo = RequestWrapper.GetParentDirectory(base.RelativeRequestPath);

			FileInfo[] _lockedFiles = _dirInfo.GetFiles("*." + base.LockToken + ".locked");
			if (_lockedFiles.Length == 0)
				base.AbortRequest(DavUnlockResponseCode.PreconditionFailed);
			else
			{
				//Delete the locked files
				_lockedFiles[0].Delete();

				//Original file path 
				string _sandBoxFileName = _lockedFiles[0].Name.Replace("." + base.LockToken + ".locked", "");
				string[] _pathInfo = _lockedFiles[0].Name.Split('_');

				//Ignore the last two
				StringBuilder _sourceFileName = new StringBuilder();
				for (int i = 0; i < _pathInfo.Length - 2; i++)
				{
					if (_sourceFileName.Length != 0)
						_sourceFileName.Append("_");

					_sourceFileName.Append(_pathInfo[i]);
				}

				//Now append the extension
				string[] _extInfo = _pathInfo[_pathInfo.Length - 1].Split('.');
				_sourceFileName.Append("." + _extInfo[1]);

				//Attempt to merge this file back to the original location
				string _baseDir = Path.Combine(Settings.Default.RepositoryPath, @"\");
				if (File.Exists(_baseDir + _sourceFileName.ToString()))
				{
					string _sandboxFilePath = _baseDir + @"sandbox\" + _sandBoxFileName;

					//Check and clear out the property file
					string _propertyFile = RequestWrapper.GetCustomPropertyInfoFilePath(_sandboxFilePath);
					if (File.Exists(_propertyFile))
						File.Delete(_propertyFile);

					File.Copy(_baseDir + @"sandbox\" + _sandBoxFileName, _baseDir + _sourceFileName.ToString(), true);
					File.Delete(_baseDir + @"sandbox\" + _sandBoxFileName);
				}
			}
		}
	}
}
