//-----------------------------------------------------------------------
// <copyright file="WebDAVFileHandler.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Web;
using Sphorium.WebDAV.Server.Framework;

namespace Sphorium.WebDAV.Examples.FileServer
{
	public class WebDAVFileHandler : IHttpHandler
	{
		private WebDavProcessor __webDavProcessor = new WebDavProcessor();

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			//Don't process WebDAV requests for the root and for files that exist (ie. default.aspx)
			string _mappedPath = context.Server.MapPath(context.Request.Url.LocalPath);
			if (!File.Exists(_mappedPath))
				__webDavProcessor.ProcessRequest(context.ApplicationInstance);
		}
	}
}
