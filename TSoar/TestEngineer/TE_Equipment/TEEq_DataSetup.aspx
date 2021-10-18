<%@ Page Title="Equipment Test Data Setup" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TEEq_DataSetup.aspx.cs"
    Inherits="TSoar.TestEngineer.TE_Equipment.TEEq_DataSetup" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Test Data Setup" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div id="divHRef" runat="server">
        <asp:Table runat="server" CellPadding="8" BorderStyle="Solid">
            <asp:TableHeaderRow>
                <asp:TableHeaderCell Text="Item" />
                <asp:TableHeaderCell Text="Action" />
                <asp:TableHeaderCell Text="Status" />
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Clean up database before adding new data" />
                <asp:TableCell>
                    <asp:Button ID="pbCleanup" runat="server" Text="Cleanup" OnClick="pbCleanup_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblCleanUpStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Generate Test Data" Font-Bold="true" ColumnSpan="3"/>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Operational Calendars" />
                <asp:TableCell >
                    <asp:Button ID="pbOpsCal" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblOpsCalStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="List of Equipment" />
                <asp:TableCell >
                    <asp:Button ID="pbEquip" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblEqStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="List of Equipment Components" />
                <asp:TableCell >
                    <asp:Button ID="pbCompon" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblComponStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Aging Parameter Sets" />
                <asp:TableCell >
                    <asp:Button ID="pbPars" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblParsStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="List of Aging Items" />
                <asp:TableCell >
                    <asp:Button ID="pbAgIt" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblAgItStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Operating Data" />
                <asp:TableCell >
                    <asp:Button ID="pbOpDat" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblOpDatStat" runat="server" Text="Not Done" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Generate All Equipment-Related Test Data" />
                <asp:TableCell >
                    <asp:Button ID="pbAll" runat="server" Text="Generate" OnClick="pbEquip_Click" />
                </asp:TableCell>
                <asp:TableCell>
                
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div id="divLbl" runat="server">
        The testing function is not available in the production version of this website.
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
