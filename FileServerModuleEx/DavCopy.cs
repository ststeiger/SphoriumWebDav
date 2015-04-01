//-----------------------------------------------------------------------
// <copyright file="DavCopy.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Sphorium.WebDAV.Server.Framework;
using Sphorium.WebDAV.Server.Framework.BaseClasses;
using Sphorium.WebDAV.Server.Framework.Resources;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavCopy.
	/// </summary>
	public sealed class DavCopy : DavCopyBase
	{
		public DavCopy()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavCopy_ProcessDavRequest);
		}

		private void DavCopy_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to make sure the resource is valid
			if (base.RelativeRequestPath == base.RequestDestination)
				base.AbortRequest(DavCopyResponseCode.Conflict);
			else if (!RequestWrapper.ValidResourceByPath(base.RelativeRequestPath))
				base.AbortRequest(ServerResponseCode.NotFound);
			else
			{
				string _moveToPath = RequestWrapper.GetResourcePath(base.RelativeRequestDestination);

				//Make sure the destination directory exists
				DirectoryInfo _destFolder = RequestWrapper.GetParentDirectory(base.RelativeRequestDestination);
				if (_destFolder == null)
					base.AbortRequest(DavCopyResponseCode.Conflict);
				else
				{
					FileInfo _sourceFile = RequestWrapper.GetFile(base.RelativeRequestPath);
					DirectoryInfo _sourceDir = RequestWrapper.GetDirectory(base.RelativeRequestPath);
					if (_sourceDir != null)
					{

						//How much do we want to copy?
						switch (base.RequestDepth)
						{
							case DepthType.ResourceOnly:
								Directory.CreateDirectory(RequestWrapper.GetResourcePath(base.RelativeRequestDestination));
								break;

							case DepthType.Infinity:
								CloneDirectory(_sourceDir, RequestWrapper.GetResourcePath(base.RelativeRequestDestination));
								break;
						}
					}
					else if (_sourceFile != null)
						_sourceFile.CopyTo(_moveToPath, base.OverwriteExistingResource);
					else
						base.AbortRequest(DavCopyResponseCode.BadGateway);
				}
			}
		}


		private void CloneDirectory(DirectoryInfo _sourceDirectory, string destination)
		{
			if (_sourceDirectory != null)
			{
				try
				{
					if (!base.OverwriteExistingResource && File.Exists(destination))
					{
						DavFolder _folderResource = new DavFolder(_sourceDirectory.Name, _sourceDirectory.Name);
						base.AddProcessErrorResource(_folderResource, DavCopyResponseCode.PreconditionFailed);
					}
					else
					{
						//Create the destination directory
						Directory.CreateDirectory(destination);

						//Move over the directory files
						foreach (FileInfo _file in _sourceDirectory.GetFiles())
							CopyFile(_file, destination + @"\" + _file.Name);

						//Clone the subdirectories
						foreach (DirectoryInfo _dir in _sourceDirectory.GetDirectories())
							CloneDirectory(_dir, destination + @"\" + _dir.Name);
					}
				}
				catch (Exception)
				{
					DavFolder _folderResource = new DavFolder(_sourceDirectory.Name, _sourceDirectory.Name);
					base.AddProcessErrorResource(_folderResource, DavCopyResponseCode.Forbidden);
				}
			}
		}

		private void CopyFile(FileInfo _sourceFile, string destination)
		{
			if (_sourceFile != null)
			{
				try
				{
					if (!base.OverwriteExistingResource && File.Exists(destination))
					{
						DavFile _fileResource = new DavFile(_sourceFile.Name, _sourceFile.Name);
						base.AddProcessErrorResource(_fileResource, DavCopyResponseCode.PreconditionFailed);
					}
					else
						_sourceFile.CopyTo(destination, base.OverwriteExistingResource);
				}
				catch (Exception)
				{
					DavFile _fileResource = new DavFile(_sourceFile.Name, _sourceFile.Name);
					base.AddProcessErrorResource(_fileResource, DavCopyResponseCode.Forbidden);
				}
			}
		}
	}
}
