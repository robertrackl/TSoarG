<%@ Page Title="TSoar Create User" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="CreateUserWizardWithRoles.aspx.cs"
    Inherits="TSoar.AdminPages.Security.CreateUserWizardWithRoles" %>
<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Web Site Adminstration - Create User" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
        <asp:CreateUserWizard ID="RegisterUserWithRoles" runat="server" ContinueDestinationPageUrl="~/AdminPages/Security/Security.aspx"
            LoginCreatedUser="False" DisplaySideBar="true"
            OnActiveStepChanged="RegisterUserWithRoles_ActiveStepChanged" CompleteSuccessText="The account has been successfully created." >
            <StepNavigationTemplate >
                <asp:Button CommandName="MoveNext" Text="Done" runat="server" />
            </StepNavigationTemplate>
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="Step 1: Enter User Account Info" >
                    <CustomNavigationTemplate>
                        <asp:Button CommandName="MoveNext" Text="Next Step" runat="server" />
                    </CustomNavigationTemplate>
                </asp:CreateUserWizardStep>
                <asp:WizardStep ID="SpecifyRolesStep" runat="server" AllowReturn="False" StepType="Step" Title="Step 2: Assign User to Role(s)" >
                    <asp:CheckBoxList ID="RoleList" runat="server">
                    </asp:CheckBoxList>
                </asp:WizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server" Title="Step 3: Creating the user is complete" >
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
