<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Translate.aspx.cs" Inherits="N2.Edit.Globalization.Translate" %>
<%@ Import Namespace="N2.Web" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="../../Resources/Css/Globalization.css" type="text/css" />
    <script type="text/javascript">
        n2ctx.toolbarSelect("globalization");
	</script>
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator Text="Cannot translate this item since the item's parent isn't translated." ID="cvCannotTranslate" meta:resourceKey="cvCannotTranslate" Display="Dynamic" CssClass="validator info" runat="server" />

</asp:Content>
