//-----------------------------------------------------------------------
// <copyright file="DavPut.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavPut.
	/// </summary>
	public sealed class DavPut : DavPutBase
	{
		public DavPut()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavPut_ProcessDavRequest);
		}

		private void DavPut_ProcessDavRequest(object sender, EventArgs e)
		{
			DirectoryInfo _dirInfo = RequestWrapper.GetParentDirectory(base.RelativeRequestPath);
			//The parent folder does not exist
			if (_dirInfo == null)
				base.AbortRequest(ServerResponseCode.NotFound);
			else
			{
				if (!base.OverwriteExistingResource)
				{
					//Check to see if the resource already exists
					if (RequestWrapper.GetFile(base.RelativeRequestPath) != null)
						base.AbortRequest(DavPutResponseCode.Conflict);
					else
						SaveFile();
				}
				else
					SaveFile();
			}
		}

		private void SaveFile()
		{
			byte[] _requestInput = base.GetRequestInput();
			using (FileStream _newFile = new FileStream(RequestWrapper.GetResourcePath(base.RelativeRequestPath), FileMode.Create))
			{
				_newFile.Write(_requestInput, 0, _requestInput.Length);
				_newFile.Close();
			}
		}
	}
}
