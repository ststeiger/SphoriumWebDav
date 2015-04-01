using System.IO;
using System.Reflection;

using Sphorium.WebDAV.Server.Framework;
using Sphorium.WebDAV.Server.Framework.Classes;
using Sphorium.WebDAV.Server.Framework.Security;

namespace Sphorium.WebDAV.Examples.FileServer
{
	/// <summary>
	/// File WebDAV Example module
	/// </summary>
	public class FileWebDAVModule : WebDAVModule
	{
		public FileWebDAVModule()
			: base(Assembly.GetExecutingAssembly(), Authentication.None)
		{
			base.ProcessRequest += new ProcessRequestEventHandler(FileWebDAVModule_ProcessRequest);
			base.BasicAuthorization += new BasicAuthorizationEventHandler(FileWebDAVModule_BasicAuthorization);
			base.DigestAuthorization += new DigestAuthorizationEventHandler(FileWebDAVModule_DigestAuthorization);
			base.AuthenticateRequest += new AuthenticationEventHandler(FileWebDAVModule_Authentication);
		}

		private void FileWebDAVModule_Authentication(object sender, AuthenticationArgs e)
		{
			e.Realm = "File WebDAV Module Example";

			//For this example, always process requests... 
			e.ProcessAuthorization = true;
		}

		private void FileWebDAVModule_DigestAuthorization(object sender, DigestAuthorizationArgs e)
		{
			//For this example, all username passwords are "test" 
			//	The password must be submitted to validate the hashed password 
			//	due to the way Digest Auth works... if the password and hash don't match, the 
			//	user will not be authorized
			e.Password = "test";
		}

		private void FileWebDAVModule_BasicAuthorization(object sender, BasicAuthorizationArgs e)
		{
			//Just for an example... authorize request IF username and password are not ""
			if (!string.IsNullOrEmpty(e.UserName) && !string.IsNullOrEmpty(e.Password))
				e.Authorized = true;
		}

		private void FileWebDAVModule_ProcessRequest(object sender, DavModuleProcessRequestArgs e)
		{
			//Process request will be false if it is not a WebDAV request
			if (e.ProcessRequest)
			{
				//Don't process WebDAV requests for the root and for files that exist (ie. default.aspx)
				string _mappedPath = base.HttpApplication.Server.MapPath(e.RequestUri.LocalPath);
				e.ProcessRequest = !File.Exists(_mappedPath);
			}
		}
	}
}