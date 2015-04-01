//-----------------------------------------------------------------------
// <copyright file="DavGet.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavGet.
	/// </summary>
	public sealed class DavGet : DavGetBase
	{
		public DavGet()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavGet_ProcessDavRequest);
		}

		private void DavGet_ProcessDavRequest(object sender, EventArgs e)
		{
			FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);
			if (_fileInfo == null)
				base.AbortRequest(ServerResponseCode.NotFound);
			else
			{
				using (FileStream _fileStream = _fileInfo.OpenRead())
				{
					long _fileSize = _fileStream.Length;
					byte[] _responseBytes = new byte[_fileSize];

					_fileStream.Read(_responseBytes, 0, (int)_fileSize);

					base.ResponseOutput = _responseBytes;

					_fileStream.Close();
				}
			}
		}
	}
}
