<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="BulkEditing.aspx.cs" Inherits="N2.Management.Content.Export.BulkEditing" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlClose">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">

<div class="tabPanel">
	<h1>Bulk editing</h1>
	<input type="hidden" id="EditableNameFilter" runat="server" />
	<asp:MultiView ID="mvWizard" runat="server" ActiveViewIndex="1">
		<asp:View ID="vNone" runat="server">
		</asp:View>
		<asp:View ID="vSelection" runat="server">
			<div class="filtering">
				<div class="formitem">
					<asp:Label runat="server" Text="Root" meta:resourceKey="lblRoot" CssClass="filteringlabel" />
					<span><%= Selection.SelectedItem.Title %></span>
				</div>
				<div class="formitem">
					<asp:Label runat="server" Text="Type" meta:resourceKey="lblType" AssociatedControlID="ddlTypes" CssClass="filteringlabel" />
					<asp:DropDownList ID="ddlTypes" DataTextField="Title" DataValueField="Discriminator" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlTypes_OnSelectedIndexChanged" />
				</div>
				<div class="formfield formitem">
					<div>
						<label><input type="radio" name="selection" value="all" <%= Request["selection"] != "specific" ? "checked='checked'" : "" %> /> All items of type <%= ddlTypes.SelectedItem.Text %> (<%= chkDescendants.Items.Count %>)</label>
					<div>
					</div>
						<label><input type="radio" name="selection" value="specific" <%= Request["selection"] == "specific" ? "checked='checked'" : "" %>/> Select specific items</label>
					</div>
					<blockquote>
						<asp:CheckBoxList CssClass="descendants" DataTextField="Title" DataValueField="ID" ID="chkDescendants" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" />
					</blockquote>
					<blockquote>
						<a href="#all" onclick="$('.descendants input').attr('checked', 'checked'); return false;">Select all</a>
						<a href="#none" onclick="$('.descendants input').removeAttr('checked'); return false;">Deselect all</a>
					</blockquote>
				</div>
				<hr />
				<asp:Button runat="server" OnCommand="OnNext" Text="Next" CssClass="btn" />
			</div>
		</asp:View>
		<asp:View ID="vEditors" runat="server">
			<div class="formitem editors">
				<asp:Label ID="lblEditors" runat="server" Text="Values to change" meta:resourceKey="lblEditors" CssClass="filteringlabel" />
				
				<asp:CheckBoxList ID="cblEditors" DataTextField="Title" DataValueField="Name" runat="server" />
			</div>
			<hr />
			<asp:Button ID="btnSelectEditors" runat="server" OnCommand="OnGotoEdit" Text="Next" CssClass="btn" />
		</asp:View>
		<asp:View ID="vEditing" runat="server">
			<asp:ValidationSummary ID="vsEdit" runat="server" CssClass="alert alert-margin" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
			<asp:CustomValidator ID="cvException" CssClass="alert alert-error alert-margin" runat="server" Display="None" />

			<n2:ItemEditor ID="ie" runat="server" />

			<hr />
			<asp:Button ID="btnSave" runat="server" OnCommand="OnSave" Text="Save" CssClass="btn btn-primary" />
		</asp:View>
		<asp:View ID="vConfirmation" runat="server">
			<asp:Repeater ID="rptAffectedItems" runat="server">
				<HeaderTemplate><h2>Affected items</h2><ul></HeaderTemplate>
				<ItemTemplate>
					<li><a href="<%# Eval("Url") %>"><img src='<%# Eval("IconUrl") %>' /><%# Eval("Title") %></a></li>
				</ItemTemplate>
				<FooterTemplate></ul></FooterTemplate>
			</asp:Repeater>
		</asp:View>
	</asp:MultiView>

</div>
<script type="text/javascript">
	$(document).ready(function () {
		$("input:radio[name='selection']").change(function () {
			var $d = $("blockquote");
			if (this.value == "specific") {
				$d.fadeIn();
			} else {
				$d.hide();
				$d.find("input").attr("checked", "checked");
			}
		}).filter(":checked").trigger("change");
	});
</script>
<style type="text/css">
	.filtering blockquote a { margin-right: 10px; color:#999; }
	.filtering .descendants label img { vertical-align:text-bottom; }
	.filtering .descendants label a { border:solid 1px silver; display:inline-block; line-height:16px; padding:0 3px; visibility:hidden; }
	.filtering .descendants label:hover a { visibility:visible; }
	.filtering .formfield { margin-left:100px; }
	
	.editors table { display:inline-block; vertical-align:top; }
	
	.filteringlabel { width:100px; display:inline-block; }
	.formitem { margin-bottom:10px; }
</style>
</asp:Content>
