<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Edit.Navigation.Search" meta:resourceKey="searchPage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Search</title>
        <asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("framed.css")%>" type="text/css" />
		</asp:PlaceHolder>
    </head>
<body class="framed navigation search">
    <form id="form1" runat="server">
        <asp:Panel runat="server" CssClass="list">
            <div id="nav" class="nav">
                <asp:GridView ID="dgrItems" runat="server" 
					DataKeyNames="ID" DataMember="Query" BorderWidth="0"
					AutoGenerateColumns="false" CssClass="table table-striped table-hover table-condensed" AlternatingRowStyle-CssClass="alt" 
					UseAccessibleHeader="true" ShowHeader="false">
                    <Columns>
						<asp:TemplateField  HeaderText="Title" meta:resourceKey="colTitle" >
                            <ItemTemplate>
                                <asp:HyperLink ID="hlShow" runat="server" Target="preview" runat="server" 
                                    NavigateUrl='<%# ((N2.INode)Container.DataItem).PreviewUrl %>'
                                    Title='<%# Eval("Published", "{0:yyy-MM-dd}") + " - " + Eval("Expires", "{0:yyy-MM-dd}") %>'
                                    rel='<%# Eval("Path") %>'>
                                    <asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" />
                                    <%# Eval("Title")%>
                                    <%# string.IsNullOrEmpty((string)Eval("ZoneName")) ? "" : Eval("ZoneName", " ({0})") %>
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
						<asp:Literal runat="server" Text="No hits" meta:resourceKey="ltNoHits" />
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
            <nav:ContextMenu id="cm" runat="server" />
            <script type="text/javascript">
            	jQuery(document).ready(function() {
            		if (window.n2ctx) window.n2ctx.toolbarSelect('search');
            	});
            </script>
        </asp:Panel>
    </form>
</body>
</html>
