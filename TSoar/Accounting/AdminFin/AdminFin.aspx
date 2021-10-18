<%@ Page Title="Accounting System Administration Overview" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="AdminFin.aspx.cs" Inherits="TSoar.Accounting.AdminFin.AdminFin" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Accounting System Administration - Overview" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li>Work with the <a href="ChrtOActs.aspx">Chart of Accounts and Subledgers</a>.</li>
        <li><a href="Banking.aspx">Banking</a>.</li>
    </ul>
    <h3>Fiscal Periods</h3>
    <div class="gvclass">
        <asp:GridView ID="gvFiscPer" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowEditing="gvFiscPer_RowEditing" OnRowCancelingEdit="gvFiscPer_RowCancelingEdit"
            OnRowDataBound="gvFiscPer_RowDataBound"
            OnRowDeleting="gvFiscPer_RowDeleting" OnRowUpdating="gvFiscPer_RowUpdating"
            HorizontalAlign="Center" ShowHeaderWhenEmpty="true"
            EnableViewState ="True" >
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Fiscal Period Description" />
                    </HeaderTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="300px" />
                    <ItemTemplate>    
                        <asp:Label runat="server" ID="lblFiscPer" Text=' <%# Eval("sPeriodDescr") %> ' />    
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbFiscPer" runat="server" text='<%# Eval("sPeriodDescr") %>' AutoCompleteType="Disabled" Width="300px" />
                    </EditItemTemplate>
                </asp:TemplateField>    
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Start Date" ToolTip="Includes the time offset to Universal Time Coordinated" />
                    </HeaderTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemTemplate>    
                        <asp:Label runat="server" ID="lblDStart" Text=' <%#  Eval("DStart") %> ' />    
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbDStart" runat="server" text='<%# Eval("DStart") %>' TextMode="Date" ToolTip="Time and UTC offset are added by the System" />
                    </EditItemTemplate>
                </asp:TemplateField>    
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="End Date" ToolTip="Includes the time offset to Universal Time Coordinated" />
                    </HeaderTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemTemplate>    
                        <asp:Label runat="server" ID="lblDEnd" Text=' <%#  Eval("DEnd") %> ' />    
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbDEnd" runat="server" text='<%# Eval("DEnd") %>' TextMode="Date" ToolTip="Time and UTC offset are added by the System" />
                    </EditItemTemplate>
                </asp:TemplateField>    
                <asp:CommandField ButtonType="Image" ShowEditButton="true" EditText="Modify" HeaderText="Modify" HeaderStyle-CssClass="text-center"
                        EditImageUrl="~/i/BlueButton.jpg" CancelImageUrl="~/i/Cancel.jpg" UpdateImageUrl="~/i/Update.jpg" >
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                </asp:CommandField>
                <asp:CommandField ButtonType="Image" DeleteText="Delete" ShowDeleteButton="True" HeaderText="Delete" HeaderStyle-CssClass="text-center"
                        DeleteImageUrl="~/i/RedButton.jpg">
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                </asp:CommandField>
            </Columns>
        </asp:GridView>
    </div>

    <div id="ModalPopupExtender">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
