//-----------------------------------------------------------------------
// <copyright file="DavVersionControl.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavVersionControlBase.
	/// </summary>
	/// <remarks>
	/// To be completed
	/// </remarks>
	public sealed class DavVersionControl : DavVersionControlBase
	{
		public DavVersionControl()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavVersionControl_ProcessDavRequest);
		}

		private void DavVersionControl_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to see if this file is already under version control
			FileInfo _fileVersionInfo = RequestWrapper.GetLatestFileVersion(base.RelativeRequestPath);

			if (_fileVersionInfo == null)
			{
				//File is currently not under version control
				FileInfo _requestFile = RequestWrapper.GetFile(base.RelativeRequestPath);

				//Tag the file with latestVersion
				if (_requestFile != null)
					_requestFile.MoveTo(_requestFile.FullName + "._.latestVersion");
				else
					base.AbortRequest(ServerResponseCode.NotFound);
			}
		}
	}
}
