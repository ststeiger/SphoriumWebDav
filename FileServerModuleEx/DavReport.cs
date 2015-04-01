//-----------------------------------------------------------------------
// <copyright file="DavReport.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Sphorium.WebDAV.Server.Framework.BaseClasses;
using Sphorium.WebDAV.Server.Framework.Classes;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavReport.
	/// </summary>
	public sealed class DavReport : DavReportBase
	{
		public DavReport()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavReport_ProcessDavRequest);
		}

		private void DavReport_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to see if this file is already under version control
			FileInfo _fileVersionInfo = RequestWrapper.GetLatestFileVersion(base.RelativeRequestPath);

			//TODO: Only populate the requested properties
			foreach (DavProperty property in base.RequestProperties)
			{
				string _here = property.Name;
			}



			//			if (_fileVersionInfo == null) {
			//				//File is currently not under version control
			//				FileInfo _requestFile = RequestWrapper.GetFile(base.RelativeRequestPath);
			//
			//				//Tag the file with latestVersion
			//				if (_requestFile != null)
			//					_requestFile.MoveTo(_requestFile.FullName + "._.latestVersion");
			//				else
			//					base.AbortRequest(ServerResponseCode.NotFound);
			//			}
		}
	}
}
