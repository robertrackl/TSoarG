<%@ Page Title="TimeAndDate Testing" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TimeAndDate.aspx.cs" Inherits="TSoar.Developer.SWLab.TimeAndDate" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="N-th occurrence of weekday in a month" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    Current date and time: <asp:Label ID="lblDateTime" runat="server" Text="DateTimeOffset"></asp:Label>
    <br /><br />
    Enter Year: <asp:TextBox ID="txbYear" runat="server" Text="2000" TextMode="Number" /><br \>
    Enter Month: <asp:TextBox ID="txbMonth" runat="server" Text="3" Textmode="Number" /><br />
    Enter nth occurrence of weekday: <asp:TextBox ID="txbNth" runat="server" Text="3" /><br />
    Select which weekday:
    <asp:DropDownList ID="DDLweekday" runat="server" >
        <asp:ListItem Text="Sunday" Value="0"></asp:ListItem>
        <asp:ListItem Text="Monday" Value="1"></asp:ListItem>
        <asp:ListItem Text="Tuesday" Value="2"></asp:ListItem>
        <asp:ListItem Text="Wednesday" Value="3"></asp:ListItem>
        <asp:ListItem Text="Thursday" Value="4"></asp:ListItem>
        <asp:ListItem Text="Friday" Value="5"></asp:ListItem>
        <asp:ListItem Text="Saturday" Value="6"></asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="pbGo" runat="server" Text="Go" OnClick="pbGo_Click" /><br />
    Result: <asp:TextBox ID="txbOut" runat="server" Width="500" Height="500" TextMode="MultiLine" />

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
