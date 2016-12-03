<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Languages.ascx.cs" Inherits="N2.Management.Myself.Languages" %>

<div class="uc">
	<h4 class="header"><%= CurrentItem.Title %></h4>
	<div class="box">

	<asp:Repeater ID="rptLanguages" runat="server">
		<HeaderTemplate>
	<table class="data">
		<thead><tr><th>Language</th><th># of items</th><th colspan="2">Changes</th></tr></thead>
		<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr><td rowspan="<%# 1 + (int)Eval("Changes.Count") %>">
				<asp:HyperLink NavigateUrl='<%# Eval("Root.Url") %>' ToolTip='<%# Eval("Language.LanguageCode")%>' runat="server">
					<span class="<%# Eval("Language.LanguageCode").ToString().Split('-').Last().ToLower() %> sprite"></span>
					<%# Eval("Language.LanguageTitle")%>
				</asp:HyperLink>
			</td><td rowspan="<%# 1 + (int)Eval("Changes.Count") %>">
				<%# Eval("TotalItems") %>
			</td>
			</tr>
				<asp:Repeater runat="server" DataSource=<%# Eval("Changes") %>>
					<ItemTemplate>
						<tr><td>
							<asp:HyperLink NavigateUrl='<%# ResolveUrl(Eval("Url")) %>' runat="server">
								<asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" />
								<%# Eval("Title")%>
							</asp:HyperLink>, 
						</td><td>
							<%# Eval("Updated")%>
						</td></tr>
					</ItemTemplate>
				</asp:Repeater>
			
		</ItemTemplate>
		<FooterTemplate>
		</tbody>
	</table>
		</FooterTemplate>
	</asp:Repeater>

</div></div>