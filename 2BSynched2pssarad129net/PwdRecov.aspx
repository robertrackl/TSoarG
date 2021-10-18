<%@ Page Title="" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="PwdRecov.aspx.cs" Inherits="TSoar.PwdRecov" %>

<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Password Recovery" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
   <div style="text-align:center">
      <div style="width: 400px; margin-left: auto; margin-right:auto;">
        <asp:PasswordRecovery
            BackColor="#F7F7DE"
            BorderColor="#CCCC99"
            BorderStyle="Solid"
            BorderWidth="1px"
            Font-Names="Verdana"
            Font-Size="10pt"
            ID="PasswordRecovery1"
            onSendMailError="PasswordRecovery1_SendMailError"
            OnSendingMail="PasswordRecovery1_SendingMail"
            runat="server"
         
            UserNameFailureText="We were unable to access your information (unknown user name?). Please try again.">
            <TitleTextStyle
                BackColor="#6B696B"
                Font-Bold="True"
                ForeColor="#FFFFFF"
                />
        </asp:PasswordRecovery>
        <p>
            <asp:TextBox ID="tbMailError" runat="server" ReadOnly="True" TextMode="MultiLine" 
                Width="757px" Font-Bold="True" Font-Size="Smaller" Height="400px" Visible="False">
                In case of an error while trying to send the email with the password, this box displays the error text.
            </asp:TextBox>
        </p>
      </div>
   </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
