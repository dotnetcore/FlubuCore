<%@ Page MasterPageFile="../../Content/Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="N2.Edit.FileSystem.Export" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%=MapCssUrl("exportImport.css")%>" type="text/css" />
</asp:Content>

<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn">Close</edit:CancelLink>
</asp:Content>

<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <n2:tabpanel id="tpImport" TabText="Import" runat="server">
    	<asp:MultiView ID="mvwImport" ActiveViewIndex="0" runat="server">
		    <asp:View runat="server">
			    <div class="upload">
				    <div class="cf">
				        <asp:FileUpload ID="fupImport" runat="server" />
				        <asp:RequiredFieldValidator ControlToValidate="fupImport" ErrorMessage="*" runat="server" />
				    </div>
				    <div>
				        <asp:Button Text="Import here" OnClick="btnImport_Click" runat="server" />
				    </div>
			    </div>
		    </asp:View>

		    <asp:View runat="server">
                <n2:hn Text="Imported files" runat="server" />
			    <asp:Repeater ID="rptImportedFiles" runat="server">
			        <ItemTemplate>
			            <div class="file"><asp:Image runat="server" ImageUrl="../../Resources/icons/page_white.png" alt="file" /><%# Container.DataItem %></div>
			        </ItemTemplate>
			    </asp:Repeater>
            </asp:View>
        </asp:MultiView>
    </n2:tabpanel>

    <n2:tabpanel id="tpExport" TabText="Export" runat="server">
        <p>
            Exporting upload directory:
            <b><asp:Literal ID="litRootPath" runat="server" /></b>
        </p>
		<div>
		    <asp:Button ID="Button1" CssClass="command" OnClick="btnExport_Click" CausesValidation="false" Text="Export files" runat="server" />
		</div>
    </n2:tabpanel>

</asp:Content>
