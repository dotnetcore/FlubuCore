<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="N2.Edit.New" Title="Create new item" meta:resourceKey="DefaultResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import namespace="N2.Definitions"%>
<asp:Content ContentPlaceHolderID="Toolbar" ID="ct" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel" CssClass="btn">Close</edit:CancelLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" ID="cc" runat="server">
	<asp:CustomValidator ID="cvPermission" CssClass="alert alert-margin" ErrorMessage="Not authorized" Display="Dynamic" runat="server" />
    <n2:TabPanel runat="server" ToolTip="Select type" meta:resourceKey="tpType">
		<div class="cf">
		<asp:PlaceHolder runat="server">
		<% for (int i = 0; i < AvailableDefinitions.Count; i++) { %>
			<div class="type cf d<%= i %> a<%= i % 2 %>">
			<% int templateIndex = 0; %>
			<% foreach(TemplateDefinition template in GetTemplates(AvailableDefinitions[i])){ %>
				<a href="<%= GetEditUrl(template.Definition) %>" style=" <%= string.IsNullOrEmpty(template.Definition.IconClass) ? string.Format("padding-left:22px; background-image:url({0})", ResolveUrl(template.Definition.IconUrl)) : "" %>" class="<%= template.Definition.TemplateKey != null ? "template" : "definition"  %> t<%= templateIndex %>">
					<b class="<%= template.Definition.IconClass %>"></b>
					<span class="title"><%= GetLocalizedString("Definitions", template.Definition.Discriminator + template.Name, "Title") ?? template.Title %></span>
					<span class="description"><%= GetLocalizedString("Definitions", template.Definition.Discriminator, "Description") ?? template.Description %></span>
				</a>
				<% templateIndex++; %>
			<% } %>
			</div>
		<% } %>
		<% if (AvailableDefinitions.Count == 0) { %>
		<em><asp:Label ID="lblNone" Text="Nothing can be created here." meta:resourceKey="lblNone" runat="server" /></em>
		<% } %>
		</asp:PlaceHolder>
		</div>
    </n2:TabPanel>
    
    <n2:TabPanel runat="server" ToolTip="Position" meta:resourceKey="tpPosition" >
		<asp:Label ID="lblPosition" runat="server" meta:resourceKey="lblZone" Text="Create new item in zone" />
        <asp:RadioButtonList ID="rblPosition" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblPosition_OnSelectedIndexChanged" CssClass="position">
            <asp:ListItem Value="0" meta:resourceKey="fsPosition_before">
                Before the selected item (at the same depth)
                <blockquote><ul>
                    <li>other items</li>
                    <li><em>new item</em></li>
                    <li><strong>selected item</strong><ul>
                        <li>other item</li>
                    </ul></li>
                </ul></blockquote>
            </asp:ListItem>
            <asp:ListItem Value="1" Selected="true" meta:resourceKey="fsPosition_below">
                Below the selected item (one level deeper)
                <blockquote><ul>
                    <li><strong>selected item</strong><ul>
                        <li>other item</li>
                        <li><em>new item</em></li>
                    </ul></li>
                    <li>other item</li>
                </ul></blockquote>
            </asp:ListItem>
        </asp:RadioButtonList>
    </n2:TabPanel>
    
    <n2:TabPanel runat="server" ToolTip="Zone" meta:resourceKey="tpZone" >
		<asp:Label ID="lblZone" runat="server" meta:resourceKey="lblZone" Text="Create new item in zone" />
		<asp:RadioButtonList ID="rblZone" DataTextField="Title" DataValueField="ZoneName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblZone_OnSelectedIndexChanged">
            <asp:ListItem Value="" Selected="true" meta:resourceKey="rblZone_default">Default</asp:ListItem>
        </asp:RadioButtonList>
    </n2:TabPanel>
    
    <script type="text/javascript">
    	var key = { up: 38, right: 39, down: 40 };
    	jQuery(document).keyup(function(e) {
    		if (e.keyCode == key.up || e.keyCode == key.down) {
    			$selectables = $(".type a");
    			var index = $selectables.index($(":focus"));
    			index += e.keyCode == key.up ? -1 : 1;
    			$selectables.eq(index).focus();
    		}
    	});
    </script>
</asp:Content>
