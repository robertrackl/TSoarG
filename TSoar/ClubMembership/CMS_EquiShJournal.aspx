<%@ Page Title="TSoar CMS_EquiShJournal" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_EquiShJournal.aspx.cs" Inherits="TSoar.ClubMembership.CMS_EquiShJournal" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equity Shares Transactions; nominal share value is $50" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <p>Club members purchase 'equity shares' in the club. Certain rights are associated with equity shares ownership as specified in the bylaws.
    In the early days of the club, the amount of one equity share purchase was $300. Later, it was increased to $350. As of 2020,
    the amount is $500. Equity shares come in units of $50, i.e., one equity share purchase of $500 means a purchase of 10 shares.</p>
    <p>There are four ways of transacting equity shares:</p>
    <ol>
        <li>Purchase: member purchases shares from the club</li>
        <li>Sale: member sells shares back to the club, also called a "repurchase"</li>
        <li>Donation: member sells shares but donates the proceeds to the club</li>
        <li>Reinstatement: shares that the member had donated are credited to the member as if purchased</li>
    </ol>
    <p>In many cases, the transaction date is not known because the transaction happened too long ago, but we know with certainty that the transaction took place;
        that is why we have the column "Date Quality".
    </p>

    <div class="gvclass">
        <asp:GridView ID="gvCMS_EquiSh" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            onrowediting="gvCMS_EquiSh_RowEditing"
            onrowdatabound="gvCMS_EquiSh_RowDataBound"
            onrowdeleting="gvCMS_EquiSh_RowDeleting"
            onrowcancelingedit="gvCMS_EquiSh_RowCancelingEdit"
            onrowupdating="gvCMS_EquiSh_RowUpdating"
            emptydatatext="<<==-- no data found --==>>"
            showheaderwhenempty="true"
            Font-Size="Small">
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHID" runat="server" Text="Internal Id"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHDname" runat="server" Text="Display Name" ToolTip="Display Name of Equity Share Owner" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDName" Text='<%# Eval("sDisplayName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLsDisplayName" runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHDXaction" runat="server" Text="Transaction Date" ToolTip="Date of Equity Shares Transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDXaction" Text='<%# ((DateTime)Eval("DXaction")).Year.ToString() + "/"
                                + ((DateTime)Eval("DXaction")).Month.ToString() + "/" + ((DateTime)Eval("DXaction")).Day.ToString() %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbDXaction" runat="server" text='<%# Eval("DXaction") %>' TextMode="Date" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHcDateQuality" runat="server" Text="Date Quality" ToolTip="The Quality of the Equity Shares Transaction Date:
 P = aPproximate
 B = Before
 X = eXact
 A = After
 G = educated Guess
 W = Wild ass guess" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblcDateQuality" Text='<%# Eval("cDateQuality") %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLcDateQuality" runat="server">
                            <asp:ListItem Text="aPproximate" Value="P" Selected="False" />
                            <asp:ListItem Text="Before" Value="B" Selected="False" />
                            <asp:ListItem Text="eXact" Value="X" Selected="True" />
                            <asp:ListItem Text="After" Value="A" Selected="False" />
                            <asp:ListItem Text="educated Guess" Value="G" Selected="False" />
                            <asp:ListItem Text="Wild ass guess" Value="W" Selected="False" />
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHdNumShares" runat="server" Text="Number of Shares" ToolTip="The number of shares involved in this transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbldNumShares" Text='<%# Eval("dNumShares") %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbdNumShares" runat="server" text='<%# Eval("dNumShares") %>'/>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbdNumShares" ValidationExpression="^[-+]?\d*\.?\d{0,4}$"
                            ErrorMessage="Must be a decimal number (optional sign, optional decimal point, up to 4 decimal places)"
                            Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHXactType" runat="server" Text="Type of Transaction" ToolTip="The type of this transaction:
 P = Purchase
 S = Sale
 D = Donation
 R = Reinstatement" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblXactType" Text='<%# Eval("cXactType") %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLcXactType" runat="server">
                            <asp:ListItem Text="Purchased by Member" Value="P" Selected="True" />
                            <asp:ListItem Text="Sold by Member" Value="S" Selected="False" />
                            <asp:ListItem Text="Donated by Member" Value="D" Selected="False" />
                            <asp:ListItem Text="Reinstated to Member" Value="R" Selected="False" />
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHsInfoSource" runat="server" Text="Source of Information"
                            ToolTip="Where did the data regarding this transaction come from? For example: application for membership; consensus knowledge of old members; Board decision; etc." />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblsInfoSource" Text='<%# Server.HtmlDecode(Convert.IsDBNull(Eval("sInfoSource")) ? "" : (string)Eval("sInfoSource")) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbsInfoSource" runat="server" text='<%# Server.HtmlDecode(Convert.IsDBNull(Eval("sInfoSource")) ? "" : (string)Eval("sInfoSource")) %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="&nbsp;Notes&nbsp;">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblsComment" Text='<%# Server.HtmlDecode(Convert.IsDBNull(Eval("sComment")) ? "" : (string)Eval("sComment")) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbsComment" runat="server" text='<%# Server.HtmlDecode(Convert.IsDBNull(Eval("sComment")) ? "" : (string)Eval("sComment")) %>'/>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:CommandField ButtonType="Image" ShowEditButton="true" HeaderText="Edit" EditImageUrl="~/i/BlueButton.jpg"
                    CancelImageUrl="~/i/Cancel.jpg" UpdateImageUrl="~/i/Update.jpg" >
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                </asp:CommandField>

                <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
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

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
