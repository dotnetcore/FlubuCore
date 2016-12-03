<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Export.Default" meta:resourcekey="PageResource1" %>
<%@ Register Src="ItemXmlImport.ascx" TagName="ItemXmlImport" TagPrefix="uc1" %>
<%@ Register Src="CsvImport.ascx" TagName="CsvImport" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%=MapCssUrl("exportImport.css")%>" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
	<edit:PersistentOnlyPanel ID="popNotSupported" runat="server" meta:resourceKey="popNotSupported">
    <n2:tabpanel runat="server" ToolTip="Import" meta:resourceKey="tpImport">
	    <asp:MultiView ID="uploadFlow" runat="server" ActiveViewIndex="0">
		    <asp:View ID="uploadView" runat="server">
		        <asp:CustomValidator id="cvImport" runat="server" CssClass="alert alert-error alert-margin" meta:resourceKey="cvImport" Display="Dynamic"/>
			    <div class="upload">
				    <div class="cf">
				        <asp:FileUpload ID="fuImport" runat="server" />
				        <asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fuImport" runat="server" ErrorMessage="*"  meta:resourceKey="rfvImport"/>
				    </div>
				    <div style="margin-top:10px;">
				        <asp:Button ID="btnVerify" runat="server" CssClass="btn btn-primary command" Text="Upload and examine" OnClick="btnVerify_Click" Display="Dynamic" meta:resourceKey="btnVerify"/>
				    </div>
			    </div>
		    </asp:View>
		    <asp:View ID="preView" runat="server">
				<uc1:ItemXmlImport id="xml" runat="server" />				
		    </asp:View>
		    <asp:View ID="csvView" runat="server">
				<uc1:CsvImport id="csv" runat="server" />
		    </asp:View>
	    </asp:MultiView>
    </n2:tabpanel>
    <n2:tabpanel id="tpExport" runat="server" ToolTip="Export" meta:resourceKey="tpExport"  NavigateUrl="Export.aspx">
    </n2:tabpanel>
	</edit:PersistentOnlyPanel>
</asp:Content>
