//-----------------------------------------------------------------------
// <copyright file="DavMove.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavMove.
	/// </summary>
	public sealed class DavMove : DavMoveBase
	{
		public DavMove()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavMove_ProcessDavRequest);
		}

		private void DavMove_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to make sure the resource is valid
			if (!RequestWrapper.ValidResourceByPath(base.RelativeRequestPath))
				base.AbortRequest(ServerResponseCode.NotFound);
			else
			{
				string _moveToPath = RequestWrapper.GetResourcePath(base.RelativeRequestDestination);

				//Make sure the destination directory exists
				DirectoryInfo _destFolder = RequestWrapper.GetParentDirectory(base.RelativeRequestPath);
				if (_destFolder == null)
					base.AbortRequest(DavMoveResponseCode.Conflict);
				else
				{
					FileInfo _sourceFile = RequestWrapper.GetFile(base.RelativeRequestPath);
					DirectoryInfo _sourceDir = RequestWrapper.GetDirectory(base.RelativeRequestPath);
					if (_sourceDir != null)
					{
						//TODO: add overwrite logic

						_sourceDir.MoveTo(_moveToPath);
					}
					else if (_sourceFile != null)
					{
						//Check to see if the file should be overwritten...
						if (File.Exists(_moveToPath) && base.OverwriteExistingResource)
						{
							//Remove any readonly flags... 
							File.SetAttributes(_moveToPath, FileAttributes.Normal);
							File.Delete(_moveToPath);
						}

						_sourceFile.MoveTo(_moveToPath);
					}
					else
						base.AbortRequest(DavMoveResponseCode.BadGateway);

				}
			}
		}
	}
}
