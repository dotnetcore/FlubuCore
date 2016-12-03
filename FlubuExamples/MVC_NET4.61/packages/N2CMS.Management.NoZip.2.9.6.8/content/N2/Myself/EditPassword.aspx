<%@ Page Language="C#" MasterPageFile="~/N2/Content/Framed.Master" AutoEventWireup="True" 
    CodeBehind="EditPassword.aspx.cs" Inherits="N2.Edit.Myself.EditPassword" Title="Change password" meta:resourcekey="PageResource1"%>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">    
    
    <style>
        label {
            width: 100px;
            float: left;
            clear: left;
        }

        .tabPanel .fields {
            width: 250px;
            display: block;
        }

            .tabPanel .fields label {
                width: 100%;
            }

        .tabPanel .field {
            display: block;
            margin-bottom: 15px;
        }

        .tabPanel .errors {
            clear: both;
            display: block;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
   <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" 
		CssClass="btn btn-primary command primary-action" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>        
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
		<asp:CustomValidator ID="cvNoUser" ErrorMessage="Only membership users may change passwords." runat="server" CssClass="alert alert-error" />
	    <div class="fields">	        
            <div class="field">
	             <asp:Label ID="lblOldPassword" Text="Old Password" AssociatedControlID="txtOldPassword" 
                    runat="server"/>
	            <asp:TextBox ID="txtOldPassword" TextMode="Password"  runat="server"/>
            </div>

            <div class="field">
	            <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" 
	                Text="New password" />
	            <asp:TextBox ID="txtPassword" TextMode="Password" runat="server"/>
            </div>
        
            <div class="field">
	            <asp:Label ID="lblRepeatPassword" runat="server" AssociatedControlID="txtRepeatPassword" 
	                Text="Repeat password"/>
	            <asp:TextBox ID="txtRepeatPassword" TextMode="Password" runat="server"/>
            </div>                        
	  </div>
        <div class="errors">
            <asp:Label  ID="lblStatusReset" runat="server"></asp:Label>
	        <br/>
	        <asp:Label  ID="lblStusRepeatPassword" runat="server"/>
        </div>
	</div>
</asp:Content>
