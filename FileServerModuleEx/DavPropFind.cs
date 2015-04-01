//-----------------------------------------------------------------------
// <copyright file="DavPropFind.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Sphorium.WebDAV.Server.Framework.BaseClasses;
using Sphorium.WebDAV.Server.Framework.Classes;
using Sphorium.WebDAV.Server.Framework.Collections;
using Sphorium.WebDAV.Server.Framework.Resources;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavPropFind.
	/// </summary>
	public sealed class DavPropFind : DavPropFindBase
	{
		public DavPropFind()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavPropFind_ProcessDavRequest);
		}

		private void DavPropFind_ProcessDavRequest(object sender, EventArgs e)
		{
			//Set the CollectionResources and DavFile
			DirectoryInfo _dirInfo = RequestWrapper.GetDirectory(base.RelativeRequestPath);

			if (_dirInfo == null)
			{
				FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);

				if (_fileInfo != null)
				{
					//TODO: handle versions

					DavFile _davFile = new DavFile(_fileInfo.Name, RequestWrapper.WebBasePath);
					_davFile.CreationDate = _fileInfo.CreationTime;
					_davFile.LastModified = _fileInfo.LastWriteTime.ToUniversalTime();


					//Check to see if there are any locks on the resource
					DavLockProperty _lockInfo = RequestWrapper.GetLockInfo(_fileInfo.FullName);
					if (_lockInfo != null)
						_davFile.ActiveLocks.Add(_lockInfo);

					//Check to see if there are any custom properties on the resource
					DavPropertyCollection _customProperties = RequestWrapper.GetCustomPropertyInfo(_fileInfo.FullName);
					if (_customProperties != null)
						_davFile.CustomProperties.Copy(_customProperties);

					_davFile.SupportsExclusiveLock = true;
					_davFile.SupportsSharedLock = true;
					_davFile.ContentLength = (int)_fileInfo.Length;

					base.FileResources.Add(_davFile);
				}
			}
			else
			{
				//Don't include any additional resources
				DavFolder _rootResource;
				if (base.RelativeRequestPath.Length == 0)
					_rootResource = new DavFolder("DavWWWRoot", RequestWrapper.WebBasePath);
				else
					_rootResource = new DavFolder(_dirInfo.Name, RequestWrapper.WebBasePath);

				_rootResource.CreationDate = _dirInfo.CreationTime.ToUniversalTime();
				_rootResource.LastModified = _dirInfo.LastWriteTime.ToUniversalTime();
				_rootResource.ContentLength = _dirInfo.GetDirectories().Length + _dirInfo.GetFiles().Length;

				base.CollectionResources.Add(_rootResource);


				//TODO: Only populate the requested properties
				switch (base.RequestDepth)
				{
					case Sphorium.WebDAV.Server.Framework.DepthType.ResourceOnly:
						break;

					default:
						foreach (DirectoryInfo _subDir in _dirInfo.GetDirectories())
						{
							//TODO: Only populate the requested properties
							DavFolder _davFolder = new DavFolder(_subDir.Name, RequestWrapper.WebBasePath + _subDir.Name);
							_davFolder.CreationDate = _subDir.CreationTime.ToUniversalTime();
							_davFolder.LastModified = _subDir.LastWriteTime.ToUniversalTime();
							_davFolder.ContentLength = _subDir.GetDirectories().Length + _subDir.GetFiles().Length;

							base.CollectionResources.Add(_davFolder);
						}

						foreach (FileInfo _fileInfo in _dirInfo.GetFiles())
						{
							//Hide all the lock / custom property information... 
							//	this is maintained in a separate file as an example
							if (_fileInfo.Extension == ".version")
							{
								//Do nothing
							}
							else if (_fileInfo.Extension == ".latestVersion")
							{
								string _fileName = RequestWrapper.ParseVersionFileName(_fileInfo);

								//TODO: Only populate the requested properties
								DavFile _davFile = new DavFile(_fileName, Path.Combine(RequestWrapper.WebBasePath, _fileName));
								_davFile.CreationDate = _fileInfo.CreationTime.ToUniversalTime();
								_davFile.LastModified = _fileInfo.LastWriteTime.ToUniversalTime();

								_davFile.SupportsExclusiveLock = true;
								_davFile.SupportsSharedLock = true;
								_davFile.ContentLength = (int)_fileInfo.Length;

								//Check to see if there are any locks on the resource
								DavLockProperty _lockInfo = RequestWrapper.GetLockInfo(_fileInfo.FullName);
								if (_lockInfo != null)
									_davFile.ActiveLocks.Add(_lockInfo);

								//Check to see if there are any custom properties on the resource
								DavPropertyCollection _customProperties = RequestWrapper.GetCustomPropertyInfo(_fileInfo.FullName);

								if (_customProperties != null)
									_davFile.CustomProperties.Copy(_customProperties);

								base.FileResources.Add(_davFile);
							}
							else if (_fileInfo.Extension != ".locked" && _fileInfo.Extension != ".property")
							{
								//TODO: Only populate the requested properties
								DavFile _davFile = new DavFile(_fileInfo.Name, Path.Combine(RequestWrapper.WebBasePath, _fileInfo.Name));
								_davFile.CreationDate = _fileInfo.CreationTime;
								_davFile.LastModified = _fileInfo.LastWriteTime.ToUniversalTime();

								_davFile.SupportsExclusiveLock = true;
								_davFile.SupportsSharedLock = true;
								_davFile.ContentLength = (int)_fileInfo.Length;


								//Check to see if there are any locks on the resource
								DavLockProperty _lockInfo = RequestWrapper.GetLockInfo(_fileInfo.FullName);
								if (_lockInfo != null)
									_davFile.ActiveLocks.Add(_lockInfo);

								//Check to see if there are any custom properties on the resource
								DavPropertyCollection _customProperties = RequestWrapper.GetCustomPropertyInfo(_fileInfo.FullName);

								if (_customProperties != null)
									_davFile.CustomProperties.Copy(_customProperties);

								base.FileResources.Add(_davFile);
							}
						}
						break;
				}
			}
		}
	}
}
