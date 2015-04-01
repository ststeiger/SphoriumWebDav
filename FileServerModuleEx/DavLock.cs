//-----------------------------------------------------------------------
// <copyright file="DavLock.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sphorium.WebDAV.Server.Framework.BaseClasses;
using Sphorium.WebDAV.Server.Framework.Classes;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavLock.
	/// </summary>
	public sealed class DavLock : DavLockBase
	{
		public DavLock()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavLock_ProcessDavRequest);
			this.RefreshLockDavRequest += new DavRefreshLockEventHandler(DavLock_RefreshLockDavRequest);
		}

		private void DavLock_ProcessDavRequest(object sender, EventArgs e)
		{
			//TODO: allow collection locking

			//Check to see if the resource exists
			FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);
			if (_fileInfo == null)
				base.AbortRequest(ServerResponseCode.NotFound);

			else
			{
				//Check to see if a lock already exists
				if (RequestWrapper.GetLockInfo(_fileInfo.FullName) != null)
					base.AbortRequest(DavLockResponseCode.Locked);
				else
				{
					string _opaqueLock = System.Guid.NewGuid().ToString("D");

					//Apply the requested lock information
					base.ResponseLock.AddLockToken(_opaqueLock);

					//Create the *.locked file
					string _lockedFilePath = _fileInfo.DirectoryName + @"\" + _fileInfo.Name + "."
						+ _opaqueLock + ".locked";

					//Serialize the lock information
					FileInfo _lockFile = new FileInfo(_lockedFilePath);
					using (Stream _lockFileStream = _lockFile.Open(FileMode.Create))
					{
						BinaryFormatter _binaryFormatter = new BinaryFormatter();
						_binaryFormatter.Serialize(_lockFileStream, base.ResponseLock);
						_lockFileStream.Close();
					}
				}
			}
		}

		private void DavLock_RefreshLockDavRequest(object sender, DavRefreshEventArgs e)
		{
			//Check to see if the lock exists
			DirectoryInfo _dirInfo = RequestWrapper.GetParentDirectory(base.RelativeRequestPath);

			FileInfo[] _lockFiles = _dirInfo.GetFiles("*." + e.LockToken + ".locked");
			if (_lockFiles.Length == 0)
				base.AbortRequest(DavLockResponseCode.PreconditionFailed);
			else
			{

				//Deserialize the lock information
				base.ResponseLock = RequestWrapper.GetLockInfo(_lockFiles[0].FullName);


				//Set the requested lockTimeout or overwrite
                if (base.ResponseLock != null)
				    base.ResponseLock.LockTimeout = 10;
			}
		}
	}
}
