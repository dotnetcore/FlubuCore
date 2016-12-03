<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Security.Default" Title="Manage Security" meta:resourcekey="PageResource1" %>
<%@ Import Namespace="N2.Edit" %>
<%@ Import Namespace="N2.Security"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server"></asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:ButtonGroup runat="server" CssClass="btn-primary">
        <asp:LinkButton ID="btnSave" runat="server" CssClass="save primary-action" data-icon-class="fa fa-save" OnCommand="btnSave_Command" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>
        <asp:LinkButton ID="btnSaveRecursive" runat="server" CssClass="command" data-icon-class="fa fa-save" OnCommand="btnSaveRecursive_Command" meta:resourcekey="btnSaveRecursiveResource1">Save whole branch</asp:LinkButton>
    </edit:ButtonGroup>
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourcekey="hlCancelResource1">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<edit:PersistentOnlyPanel ID="popNotSupported" runat="server" meta:resourceKey="popNotSupported">
	<edit:PermissionPanel id="ppPermitted" runat="server" meta:resourceKey="ppPermitted">
    <asp:CustomValidator ID="cvSomethingSelected" runat="server" Display="Dynamic" CssClass="alert alert-margin" Text="" ErrorMessage="At least one role must be selected" OnServerValidate="cvSomethingSelected_ServerValidate" meta:resourcekey="cvSomethingSelectedResource1" />
    <style>
		.defaults td { border-bottom:solid 1px #ccc;}
		.permissionsHeader { width:130px; }
		td { width:65px;}
		.AuthorizedFalse { opacity:.33; }
    </style>
    <script type="text/javascript">
	    $(document).ready(function() {
		    $.fn.disable = function() {
			    return this.attr("disabled", "disabled");
		    };
		    $.fn.enable = function() {
			    return this.removeAttr("disabled");
		    };
		    var updateColumn = function() {
			    var groupName = this.parentNode.className.split(' ')[1];
			    var $grouped = $("." + groupName + " input").not(this);
			    if (this.checked) {
				    $grouped.parent().andSelf().disable();
			    } else {
				    $grouped.parent().andSelf().enable();
			    }
			    var $unauthorized = $grouped.parent().filter(".AuthorizedFalse").children("input").andSelf();
			    $unauthorized.disable();
			    return $grouped;
		    };
		    $(".overrides .cb input").filter(":checked").addClass("defaultChecked");
		    $(".defaults .cb input").click(function() {
			    var $grouped = updateColumn.call(this);
			    $grouped.filter(".defaultChecked").attr("checked", true);
			    $grouped.filter(":not(.defaultChecked)").removeAttr("checked");
		    }).each(updateColumn);
	    });
    </script>
<div class="tabPanel">
    <table>
		<thead>
			<tr>
				<td class="permissionsHeader" title="Altered: <%= Selection.SelectedItem.AlteredPermissions %>"></td>
				<asp:Repeater ID="rptHeaders" runat="server" DataSource="<%# Permissions %>"><ItemTemplate>
					<td><%# Container.DataItem %></td>
				</ItemTemplate></asp:Repeater>
			</tr>
		</thead>
		
		<tbody class="defaults">
			<tr>
				<td><%= GetLocalResourceString("DefaultText", "Default")%></td>
			<asp:Repeater ID="rptEveryone" runat="server" DataSource="<%# Permissions %>"><ItemTemplate>
				<td>
					<asp:CheckBox ID="cbEveryone" Checked="<%# IsEveryone((Permission)Container.DataItem) %>" Enabled="<%# IsAuthorized(Selection.SelectedItem, (Permission)Container.DataItem) %>" runat="server" CssClass='<%# "cb permission" + Container.ItemIndex %>' />
					<asp:CustomValidator ID="cvMarker" ErrorMessage="<%# Container.DataItem.ToString() %>" Text="*" runat="server" />
				</td>
			</ItemTemplate></asp:Repeater>
			</tr>
		</tbody>
		
		<tbody class="overrides">
		<asp:Repeater ID="rptPermittedRoles" runat="server" DataSource="<%# GetAvailableRoles() %>"><ItemTemplate>
			<tr>
				<td><%# Container.DataItem %></td>
				<asp:Repeater ID="rptPermissions" runat="server" DataSource="<%# Permissions %>" OnItemCreated="rptPermissions_ItemCreated"><ItemTemplate>
					<td>
						<asp:CheckBox ID="cbRole" runat="server" 
									  Checked="<%# IsRolePermitted(GetRole(Container), (Permission)Container.DataItem) %>" 
									  CssClass='<%# "cb permission" + Container.ItemIndex + " Authorized" + IsUserPermitted(GetRole(Container), (Permission)Container.DataItem) %>' />
					</td>
				</ItemTemplate></asp:Repeater>
			</tr>	
		</ItemTemplate></asp:Repeater>		
		</tbody>
    </table>
</div>
</edit:PermissionPanel>
</edit:PersistentOnlyPanel>
</asp:Content>
