//-----------------------------------------------------------------------
// <copyright file="DavMkCol.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

using Sphorium.WebDAV.Server.Framework.BaseClasses;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavMKCol.
	/// </summary>
	public sealed class DavMKCol : DavMKColBase
	{
		public DavMKCol()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavMKCol_ProcessDavRequest);
		}

		private void DavMKCol_ProcessDavRequest(object sender, EventArgs e)
		{
			//Check to see if the RequestPath is already a resource
			if (RequestWrapper.ValidResourceByPath(base.RelativeRequestPath))
				base.AbortRequest(DavMKColResponseCode.MethodNotAllowed);
			else
			{
				//Check to see if the we can create a new folder
				DirectoryInfo _dirInfo = RequestWrapper.GetParentDirectory(base.RelativeRequestPath);

				//The parent folder does not exist
				if (_dirInfo == null)
					base.AbortRequest(DavMKColResponseCode.Conflict);
				else
				{
					string _requestedFolder = RequestWrapper.GetResourceName(base.RelativeRequestPath);

					try
					{
						//Create the subFolder
						if (_dirInfo.CreateSubdirectory(_requestedFolder) == null)
							base.AbortRequest(DavMKColResponseCode.Forbidden);
						else
						{
							//TODO: provide support for requestbody
							if (base.RequestLength != 0)
							{

							}
						}
					}
					catch (Exception)
					{
						base.AbortRequest(DavMKColResponseCode.Forbidden);
					}
				}
			}
		}
	}
}
