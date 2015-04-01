<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Sphorium.WebDAV.Examples.FileServer.Properties" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
	private string WebDAVRepositoryRoot;
	private string WebDAVSSLRepositoryRoot; 

	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.Request.IsSecureConnection)
		{
			this.WebDAVSSLRepositoryRoot = this.Request.Url.GetLeftPart(UriPartial.Authority) + this.Request.ApplicationPath;
			if (!this.WebDAVSSLRepositoryRoot.EndsWith("/"))
				this.WebDAVSSLRepositoryRoot += "/";
			
			this.WebDAVRepositoryRoot = this.WebDAVRepositoryRoot.Replace("https://", "http://");
		}
		else
		{
			this.WebDAVRepositoryRoot = this.Request.Url.GetLeftPart(UriPartial.Authority) + this.Request.ApplicationPath;
			if (!this.WebDAVRepositoryRoot.EndsWith("/"))
				this.WebDAVRepositoryRoot += "/";
			
			this.WebDAVSSLRepositoryRoot = this.WebDAVRepositoryRoot.Replace("http://", "https://");
		}
		
		List<FileInfo> _fileList = new List<FileInfo>();
		DirectoryInfo _dirInfo = new DirectoryInfo(Settings.Default.RepositoryPath);

		foreach (FileInfo _fileInfo in _dirInfo.GetFiles())
		{
			switch (_fileInfo.Extension)
			{
				case ".version":
				case ".latestVersion":
				case ".locked":
				case ".property":
					break;

				case ".doc":
				case ".docx":
				case ".ppt":
				case ".pptx":
				case ".xls":
				case ".xlsx":
					_fileList.Add(_fileInfo);
					break;
			}
		}

		fileInfo.DataSource = _fileList;
		//fileInfo.DataBind();

		this.DataBind();
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>WebDAV.NET Framework Example</title>

	<script src="<%= this.ResolveUrl("core.js") %>" type="text/javascript"></script>

	<style type="text/css">
		body
		{
			font-family: Tahoma;
			font-size: 13px;
		}
		
		h4
		{
			font-family: Tahoma;
			font-size: 14px;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<h4 align="center">
			This is a simple example on how to use the WebDAV Server framework<br />
		</h4>
		<p align="center">
			This example utilizes the Sphorium.WebDAV.Examples.FileServer.Module<br />
			which implements the Sphorium.WebDAV.Server.Framework (WebDAV.NET framework).<br />
			<br />
			All the compiled assemblies and their source is available at <a href="http://webdav.sourceforge.net">
				http://webdav.sourceforge.net </a>
		</p>
		<table cellpadding="5" cellspacing="5" width="100%">
			<tr>
				<td colspan="2">
					Direct File Access:
				</td>
			</tr>
			<tr>
				<td width="10px">
				</td>
				<td>
					Click on the following links below to access and update Microsoft Office documents
					via WebDAV (requires IE)
					<br />
					<br />
					WebDAV Files (click to access the file... only office documents are displayed)
					<br />
					<br />
					<asp:Repeater ID="fileInfo" runat="server">
						<HeaderTemplate>
							<table border="1" style="border: 1px solid black; border-collapse: collapse; width: 700px;">
								<tr>
									<th>
										File
									</th>
									<th>
										Secure File (via SSL)
									</th>
									<th>
										Size
									</th>
									<th>
										Last Modified
									</th>
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td align="center">
									<a onclick="editDocumentWithProgID('<%# this.WebDAVRepositoryRoot + ((FileInfo)Container.DataItem).Name %>', 2)"
										href="javascript:void(0)">
										<%# ((FileInfo)Container.DataItem).Name %></a>
								</td>
								<td align="center">
									<a onclick="editDocumentWithProgID('<%# this.WebDAVSSLRepositoryRoot + ((FileInfo)Container.DataItem).Name %>', 2)"
										href="javascript:void(0)">
										<%# ((FileInfo)Container.DataItem).Name %></a>
								</td>
								<td align="center">
									<%# ((FileInfo)Container.DataItem).Length %>
									bytes
								</td>
								<td align="center">
									<%# ((FileInfo)Container.DataItem).LastWriteTime.ToString("MM/dd/yy HH:mm:ss") %>
								</td>
							</tr>
						</ItemTemplate>
						<FooterTemplate>
							</tr> </table>
						</FooterTemplate>
					</asp:Repeater>
				</td>
			</tr>
			<tr>
				<td height="20px">
				</td>
			</tr>
			<tr>
				<td colspan="2">
					Web Folder Access:
				</td>
			</tr>
			<tr>
				<td width="10px">
				</td>
				<td>
					To modify the WebDAV repository (add files, folders, rename, delete, search, edit
					Word documents, etc.):
				</td>
			</tr>
			<tr>
				<td>
				</td>
				<td>
					- Map Network Drive:<br />
					&nbsp;&nbsp;Map a network drive to <%# this.WebDAVRepositoryRoot %> or <%# this.WebDAVSSLRepositoryRoot %> (if SSL is enabled)
					<br />
					<br />
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
