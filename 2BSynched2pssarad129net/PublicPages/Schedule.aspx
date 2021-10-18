<%@ Page Title="TSoar Schedule/Calendar" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs"
    Inherits="TSoar.PublicPages.Schedule" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The PSSA Calendar of Operations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Anybody can see our schedule of operations. The Operations Manager 
            maintains the schedule. In order to sign up for an operation, PSSA members use credentials given to them 
            by the website adminstrator via invitation to this website. 
            Anonymous or non-member sign-ups or reservations are not possible - please see our 
            <asp:HyperLink runat="server" NavigateUrl="./Contact.aspx">contacts</asp:HyperLink> page where you can leave a voicemail <%-- // SCR 221 --%>
            or send us an email with your request.</p> <%-- // SCR 221 --%>
        <p>All operations take place at Bergseth Field in Enumclaw, Washington, unless the Notes for a day of operations say otherwise.</p>
    </div><%-- SCR 213 --%>
    <div class="gvclass">
        <asp:GridView ID="gvOpsSch" runat="server" AutoGenerateColumns="False"
                GridLines="None" CssClass="SoarNPGridStyle" EmptyDataText="--==>> No Data Found <<==--"
                OnRowDataBound="gvOpsSch_RowDataBound"
                OnPageIndexChanging="gvOpsSch_PageIndexChanging" AllowPaging="true" PageSize="35">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>

                <asp:TemplateField Visible="false">
                    <HeaderTemplate>
                        Internal Date ID
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDate" runat="server" Text='<%# Eval("ID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHDate" runat="server" Text="Date" ToolTip="The Date of the Day of Operations" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTime)Eval("DDate")),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHDoW" runat="server" Text="Day of Week" ToolTip="The day of the week of the date to the left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblDoW" runat="server" Text='<%# ((DateTime)Eval("DDate")).DayOfWeek %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHNotes" runat="server" Text="Notes" ToolTip="Notes for Day of Operations" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("sNote") %>' ToolTip="All flights from and to Bergseth Field, Enumclaw, Washington, except when noted otherwise" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[1].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl01" runat="server" Text='<%# Eval(dictColNames[1].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[2].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl02" runat="server" Text='<%# Eval(dictColNames[2].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[3].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl03" runat="server" Text='<%# Eval(dictColNames[3].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[4].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl04" runat="server" Text='<%# Eval(dictColNames[4].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[5].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl05" runat="server" Text='<%# Eval(dictColNames[5].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[6].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl06" runat="server" Text='<%# Eval(dictColNames[6].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[7].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl07" runat="server" Text='<%# Eval(dictColNames[7].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[8].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl08" runat="server" Text='<%# Eval(dictColNames[8].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[9].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl09" runat="server" Text='<%# Eval(dictColNames[9].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[10].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl10" runat="server" Text='<%# Eval(dictColNames[10].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[11].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl11" runat="server" Text='<%# Eval(dictColNames[11].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[12].sCateg %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl12" runat="server" Text='<%# Eval(dictColNames[12].sCateg) %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[13].sCateg %> <%-- // SCR 222 --%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl13" runat="server" Text='<%# Eval(dictColNames[13].sCateg) %>' /> <%-- // SCR 222 --%>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[14].sCateg %> <%-- // SCR 222 --%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl14" runat="server" Text='<%# Eval(dictColNames[14].sCateg) %>' /> <%-- // SCR 222 --%>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# dictColNames[15].sCateg %> <%-- // SCR 222 --%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbl15" runat="server" Text='<%# Eval(dictColNames[15].sCateg) %>' /> <%-- // SCR 222 --%>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
