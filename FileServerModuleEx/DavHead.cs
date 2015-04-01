//-----------------------------------------------------------------------
// <copyright file="DavHead.cs" company="Sphorium Technologies">
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
	/// Custom implementation example for DavHead.
	/// </summary>
	public sealed class DavHead : DavHeadBase
	{
		public DavHead()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavHead_ProcessDavRequest);
		}

		private void DavHead_ProcessDavRequest(object sender, EventArgs e)
		{
			if (!RequestWrapper.ValidResourceByPath(base.RelativeRequestPath))
				base.AbortRequest(ServerResponseCode.NotFound);

			else
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

					//Set the resource
					base.Resource = _davFile;
				}
			}
		}
	}
}
