<%@ Import Namespace="N2.Edit.Wizard.Items" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Wizards.ascx.cs" Inherits="N2.Management.Myself.Wizards" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<div class="uc">
	<h4 class="header"><%= CurrentItem.Title %></h4>
	<div class="box">

	<n2:Repeater ID="rptLocations" runat="server">
		<HeaderTemplate><table class="data"><thead><tr><th colspan="2">Wizard</th><th>Location</th></tr></thead><tbody></HeaderTemplate>
		<ItemTemplate>
			<tr><td>
				<edit:ItemLink DataSource="<%# Container.DataItem %>" runat="server" InterfaceUrl="{ManagementUrl}/Content/Edit.aspx" />
			</td><td>
				<%# Eval("Description") %>
			</td><td>
				<edit:ItemLink DataSource='<%# Eval("Location") %>' runat="server" />
			</td></tr>
		</ItemTemplate>
		<EmptyTemplate>
			<div class="inner"><asp:Label runat="server" ID="lblNoItems" meta:resourcekey="lblNoItems" Text="No locations added." /></div>
		</EmptyTemplate>
		<FooterTemplate></tbody></table></FooterTemplate>
	</n2:Repeater>

</div></div>