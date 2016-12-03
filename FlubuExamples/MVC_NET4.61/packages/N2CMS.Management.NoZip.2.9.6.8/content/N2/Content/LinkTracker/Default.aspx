<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.LinkTracker._Default" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
    <table><tbody><tr><td>
	<edit:FieldSet class="referencingItems" runat="server" Legend="Incoming links" meta:resourceKey="referencingItems">
		<div style="margin:10px;min-height:15px;">
		<asp:Repeater runat="server" ID="rptReferencingItems">
			<ItemTemplate>
				<div>
					<edit:ItemLink InterfaceUrl="Default.aspx" DataSource="<%# Container.DataItem %>" runat="server" />
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<div style="margin:10px">
	</edit:FieldSet>
	</td><td style="padding:22px 10px;">
		<b style="font-size:24px" class="fa fa-long-arrow-right"></b>
	</td><td style="padding:22px 10px;">
		<edit:ItemLink InterfaceUrl="Default.aspx" DataSource="<%# Selection.SelectedItem %>" runat="server" />
	</td><td style="padding:22px 10px;">
		<b style="font-size:24px" class="fa fa-long-arrow-right"></b>
	</td><td>
	<edit:FieldSet class="referencedItems" runat="server" Legend="Outgoing links" meta:resourceKey="referencedItems">
		<div style="margin:10px;min-height:15px;">
		<asp:Repeater runat="server" ID="rptReferencedItems">
			<ItemTemplate>
				<div>
					<edit:ItemLink InterfaceUrl="Default.aspx" DataSource="<%# Container.DataItem %>" runat="server" />
				</div>
			</ItemTemplate>
		</asp:Repeater>	
		</div>
	</edit:FieldSet>
	</td></tr></tbody></table>
	</div>
</asp:Content>
