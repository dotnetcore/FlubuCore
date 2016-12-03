<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="N2.Management.Content.Export.Export" meta:resourcekey="PageResource1" %>
<%@ Register Src="../AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%=MapCssUrl("exportImport.css")%>" type="text/css" />
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<edit:PersistentOnlyPanel ID="popNotSupported" runat="server" meta:resourceKey="popNotSupported">
    <n2:tabpanel id="tpImport" runat="server" TabText="Import" meta:resourceKey="tpImport" NavigateUrl="Default.aspx">
    </n2:tabpanel>
    <n2:tabpanel id="tpExport" runat="server" TabText="Export" Selected="true" meta:resourceKey="tpExport">
		<div>
		    <asp:CheckBox ID="chkDefinedDetails" runat="server" Text="Exclude computer generated data"  meta:resourceKey="chkDefinedDetails" />
		</div>
		<div>
		    <asp:CheckBox ID="chkAttachments" runat="server" Text="Don't export attachments"  meta:resourceKey="chkAttachments" />
		</div>
		<div style="margin-top:10px;">
		    <asp:Button ID="btnExport" runat="server" CssClass="btn btn-primary command" OnCommand="btnExport_Command" CausesValidation="false" meta:resourceKey="btnExport" Text="Export these items" />
		</div>
		<hr />
		<n2:hn runat="server" Level="3" Text="Exported items" meta:resourceKey="exportedItems" />
		<uc1:AffectedItems id="exportedItems" runat="server" />		
    </n2:tabpanel>
	</edit:PersistentOnlyPanel>	
</asp:Content>
