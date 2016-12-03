<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsEditor.ascx.cs" Inherits="N2.Edit.Settings.SettingsEditor" %>
<div>
    <asp:CheckBox runat="server" ID="chkShowDataItems" Text="Show non-pages in navigation" />
</div>
<div>
    <asp:Label runat="server" AssociatedControlID="ddlThemes" Text="Admin theme" />
    <asp:DropDownList runat="server" ID="ddlThemes" DataTextField="Name" DataValueField="Name" />
</div>