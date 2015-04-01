//-----------------------------------------------------------------------
// <copyright file="DavOptions.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavOptions.
	/// </summary>
	public sealed class DavOptions : DavOptionsBase
	{
		/// <summary>
		/// Dav HttpRequest OPTIONS Framework Base Class
		/// </summary>
		public DavOptions()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavOptions_ProcessDavRequest);
		}

		private void DavOptions_ProcessDavRequest(object sender, EventArgs e)
		{
			//Provide support for all
			base.SupportedHttpMethods = HttpMethods.All;
		}
	}
}