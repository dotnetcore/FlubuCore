<%@ Control
	Language="C#"
	CodeBehind="AffectedItems.ascx.cs"
	Inherits="N2.Edit.AffectedItems"
	meta:resourceKey="AffectedItemsResource" %>
<%@ Register
	TagPrefix="edit"
	Namespace="N2.Edit.Web.UI.Controls"
	Assembly="N2.Management" %>
<div id="nav" class="tree nav">
	<edit:Tree
		ID="tv"
		runat="server"
		Target="_blank" />
</div>
