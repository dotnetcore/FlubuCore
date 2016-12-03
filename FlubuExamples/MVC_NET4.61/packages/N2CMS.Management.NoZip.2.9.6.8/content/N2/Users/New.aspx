<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="N2.Edit.Membership.New" Title="New user" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:HyperLink runat="server" NavigateUrl="Users.aspx" CssClass="btn command" 
		meta:resourcekey="HyperLinkResource1">Close</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
    <% if(!IsMembershipAccountType()) { %>
        <div class="warning">Adding a new user is supported for classic Membership only. See "logout and register as a new user". </div>
    <% } else { %>
	<asp:CreateUserWizard ID="createUserWizard" runat="server" 
		OnCreatedUser="createUserWizard_CreatedUser" 
		OnContinueButtonClick="createUserWizard_FinishButtonClick" 
		LoginCreatedUser="False" meta:resourcekey="createUserWizardResource1">
		<WizardSteps>
			<asp:CreateUserWizardStep ID="cuwsCreate" runat="server" 
				meta:resourcekey="cuwsCreateResource1">
				<ContentTemplate>
					<div>
						<asp:Label ID="lblUserName" runat="server" AssociatedControlID="UserName" 
							meta:resourcekey="lblUserNameResource1">User name</asp:Label>
						<asp:TextBox ID="UserName" runat="server" 
							meta:resourcekey="UserNameResource2" />
					</div>
					<div>
						<asp:Label ID="lblPassword" runat="server" AssociatedControlID="Password" 
							meta:resourcekey="lblPasswordResource1">Password</asp:Label>
						<asp:TextBox ID="Password" runat="server" TextMode="Password" meta:resourcekey="PasswordResource2" />
					</div>
					<div>
						<asp:Label ID="lblEmail" runat="server" AssociatedControlID="Email" 
							meta:resourcekey="lblEmailResource1">Email</asp:Label>
						<asp:TextBox ID="Email" runat="server" meta:resourcekey="EmailResource2" />
					</div>
					<asp:TextBox ID="Question" runat="server" Visible="False" meta:resourcekey="QuestionResource1" />
					<asp:TextBox ID="Answer" runat="server" Visible="False" meta:resourcekey="AnswerResource1" />
					<div>
						<asp:Label ID="lblRoles" runat="server" AssociatedControlID="cblRoles" 
							meta:resourcekey="lblRolesResource1">Roles</asp:Label>
						<div class="checkBoxList">
							<asp:CheckBoxList ID="cblRoles" runat="server" CssClass="cbl" 
							DataSourceID="odsRoles" meta:resourcekey="cblRolesResource1" RepeatLayout="Flow" />
						</div>
						<asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="N2.Edit.Membership.RolesSource" SelectMethod="GetAllRoles" />
					</div>
				</ContentTemplate>
			</asp:CreateUserWizardStep>
			<asp:CompleteWizardStep runat="server">
			</asp:CompleteWizardStep>
		</WizardSteps>
	</asp:CreateUserWizard>
    <% } %>
	</div>
</asp:Content>
