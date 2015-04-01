//-----------------------------------------------------------------------
// <copyright file="DavDelete.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavDeleteBase
	/// </summary>
	public sealed class DavDelete : DavDeleteBase
	{
		public DavDelete()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavDelete_ProcessDavRequest);
		}

		private void DavDelete_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to see if the resource is a folder
			DirectoryInfo _dirInfo = RequestWrapper.GetDirectory(base.RelativeRequestPath);

			if (_dirInfo != null)
			{
				//TODO... Attempt to delete the subfolders
				try
				{
					_dirInfo.Delete(true);
				}
				catch (Exception) { }
			}
			else
			{
				FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);
				if (_fileInfo != null)
				{
					string _propFilePath = RequestWrapper.GetCustomPropertyInfoFilePath(_fileInfo.FullName);
					if (File.Exists(_propFilePath))
						File.Delete(_propFilePath);

					string _lockFilePath = RequestWrapper.GetLockInfoFilePath(_fileInfo.FullName);
					if (File.Exists(_lockFilePath))
						File.Delete(_lockFilePath);

					_fileInfo.Delete();
				}
			}
		}
	}
}
