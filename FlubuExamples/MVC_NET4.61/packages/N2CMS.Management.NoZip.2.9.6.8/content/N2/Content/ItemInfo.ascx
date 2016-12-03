<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemInfo.ascx.cs" Inherits="N2.Edit.ItemInfo" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<n2:Box ID="boxInfo" HeadingText="Info" CssClass="box infoBox" runat="server" meta:resourceKey="boxInfo">
	<edit:InfoLabel Label="ID"	        Text="<%# CurrentItem.ID %>" runat="server" meta:resourceKey="ilID"/>
	<edit:InfoLabel Label="Type"		Text="<%# CurrentDefinition.Title %>" runat="server" meta:resourceKey="ilType"/>
	<edit:InfoLabel Label="Version"		Text="<%# CurrentItem.VersionIndex + 1 %>" runat="server" meta:resourceKey="ilVersion"/>
	<edit:InfoLabel Label="State"		Text="<%# GetStateText(CurrentItem) %>" runat="server" meta:resourceKey="ilState"/>
	<edit:InfoLabel Label="Template"		Text="<%# CurrentItem.TemplateKey %>" runat="server" meta:resourceKey="ilTemplate"/>
	<edit:InfoLabel Label="Published"	Text="<%# CurrentItem.Published %>" runat="server" meta:resourceKey="ilPublished"/>
	<edit:InfoLabel Label="Expires"		Text="<%# CurrentItem.Expires %>" runat="server" meta:resourceKey="ilExpires"/>
</n2:Box>