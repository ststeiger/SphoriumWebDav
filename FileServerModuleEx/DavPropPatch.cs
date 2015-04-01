//-----------------------------------------------------------------------
// <copyright file="DavPropPatch.cs" company="Sphorium Technologies">
//     Copyright (c) Sphorium Technologies. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sphorium.WebDAV.Server.Framework.BaseClasses;
using Sphorium.WebDAV.Server.Framework.Classes;
using Sphorium.WebDAV.Server.Framework.Collections;
using Sphorium.WebDAV.Server.Framework.Resources;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// Custom implementation example for DavPropPatch.
	/// </summary>
	public sealed class DavPropPatch : DavPropPatchBase
	{
		public DavPropPatch()
		{
			this.ProcessDavRequest += new DavProcessEventHandler(DavPropPatch_ProcessDavRequest);
			this.PostProcessDavRequest += new DavRequestEventHandler(DavPropPatch_PostProcessDavRequest);
		}

		private void DavPropPatch_ProcessDavRequest(object sender, EventArgs e)
		{

			//Check to see if we can replace the properties

			//			//Fake out the delete process issue by default
			//			DavFile _junk = new DavFile();
			//			_junk.FilePath = "http://www.bubba.com/nope";
			//
			//			base.AddProcessErrorResource(_junk, DavDeleteResponseCode.Locked);
			//			base.AddProcessErrorResource(_junk, DavDeleteResponseCode.InsufficientStorage);
			//
			//			return;


			//Logic... if anything fails... must rollback!

			//Check to see if the resource exists
			FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);
			if (_fileInfo != null)
			{
				DavFile _davFile = new DavFile(_fileInfo.Name, RequestWrapper.WebBasePath);
				base.PatchedResource = _davFile;

				//Check to see if there are any custom properties on the resource
				DavPropertyCollection _customProperties = RequestWrapper.GetCustomPropertyInfo(_fileInfo.FullName);
				if (_customProperties == null)
					_customProperties = new DavPropertyCollection();

				//Update the delta's... remove the deleted properties
				foreach (DavProperty _property in base.RequestDeleteProperties)
					_customProperties.Remove(_property.Name);

				foreach (DavProperty _property in base.RequestModifyProperties)
				{
					DavProperty _customProperty = _customProperties[_property.Name];
					if (_customProperty != null)
						_customProperties.Remove(_property.Name);

					_customProperties.Add(_property);
				}

				_davFile.CustomProperties.Copy(_customProperties);
			}
		}

		private void DavPropPatch_PostProcessDavRequest(object sender, EventArgs e)
		{
			//Check to make sure the request succeeded
			if (base.HttpResponseCode == (int)ServerResponseCode.MultiStatus)
			{
				//Serialize the custom properties
				FileInfo _fileInfo = RequestWrapper.GetFile(base.RelativeRequestPath);
				if (_fileInfo != null)
				{
					//Check to see if there are any custom properties on the resource
					DavPropertyCollection _customProperties = base.PatchedResource.CustomProperties;
					string _propFilePath = RequestWrapper.GetCustomPropertyInfoFilePath(_fileInfo.FullName);
					if (_customProperties.Count == 0)
					{
						//Delete the file if there are no custom properties
						if (File.Exists(_propFilePath))
							File.Delete(_propFilePath);
					}
					else
					{
						//Create / update the *.property file
						FileInfo _propFile = new FileInfo(_propFilePath);
						using (Stream _propFileStream = _propFile.Open(FileMode.Create))
						{
							BinaryFormatter _binaryFormatter = new BinaryFormatter();
							_binaryFormatter.Serialize(_propFileStream, _customProperties);
							_propFileStream.Close();
						}
					}
				}
			}
		}
	}
}
