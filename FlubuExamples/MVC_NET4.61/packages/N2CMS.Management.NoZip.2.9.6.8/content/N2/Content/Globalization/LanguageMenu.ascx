<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="LanguageMenu.ascx.cs" Inherits="N2.Edit.Globalization.LanguageMenu" %>
<asp:PlaceHolder ID="plhNew" runat="server" Visible='<%# CreatingNew %>'>
	<asp:HyperLink Enabled="false" ID="hlNew" runat="server" CssClass="command" NavigateUrl='<%# "Default.aspx?" + N2.Edit.SelectionUtility.SelectedQueryKey + "=" + Server.UrlEncode(Selection.SelectedItem.Path) %>' ToolTip="<%# CurrentLanguage.LanguageCode %>">
		<span class="<%# CurrentLanguage.LanguageCode.Split('-').LastOrDefault().ToLower() %> sprite"></span>
		<%# CurrentLanguage.LanguageTitle %>
	</asp:HyperLink>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" Visible='<%# !CreatingNew %>'>
	<n2:OptionsMenu id="om" runat="server">
		<asp:HyperLink runat="server" CssClass="command plain globalize" NavigateUrl='<%# "Default.aspx?" + N2.Edit.SelectionUtility.SelectedQueryKey + "=" + Server.UrlEncode(Selection.SelectedItem.Path) %>' ToolTip="<%# CurrentLanguage.LanguageCode %>">
			<span class="<%# CurrentLanguage.LanguageCode.Split('-').LastOrDefault().ToLower() %> sprite"></span>
			<%# CurrentLanguage.LanguageTitle %>
		</asp:HyperLink>
		<asp:Repeater runat="server" id="rptLanguages">
			<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# Eval("EditUrl") %>' CssClass="command plain" runat="server" ToolTip='<%# Eval("Language.LanguageCode") %>'>
					<span class="<%# Eval("Language.LanguageCode").ToString().Split('-').LastOrDefault().ToLower() %> sprite"></span>
					<%# Eval("Language.LanguageTitle") %>
				</asp:HyperLink>
			</ItemTemplate>
		</asp:Repeater>
	</n2:OptionsMenu>
</asp:PlaceHolder>
