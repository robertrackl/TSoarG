<%@ Page Title="Rate Sheet" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="RateSheet.aspx.cs" Inherits="TSoar.PublicPages.RateSheet" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Rate Sheet" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .rstable {width:100%; max-width: 600px;}
        .rsrightalign {text-align: right;}
    </style>
    <div class="HelpText" ><%-- SCR 213 --%>
        <h3 class="text-center">Current Membership, Glider & Tow Rates</h3><%-- SCR 213 --%>
        <h3 class="text-center">Effective May 1, 2021</h3><%-- SCR 213 --%>
        <p>Note: Due to the weight & balance limitations of the gliders, passenger weight is limited to at most 235 lbs.
        </p>
        <h4>Club Fees</h4>
        <asp:Table CssClass="rstable" runat="server" BorderWidth="2px">
            <asp:TableRow>
                <asp:TableCell>Introductory Membership for 30 Days; includes one demonstration flight; additional flights may be purchased at
                    Flight-Related Fees listed below
                </asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$150.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Initiation Fee</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$50.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Club Share</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$500.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Annual Dues - Regular Active Members*</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$960.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Annual Dues - Family Members and Youth Members*</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$480.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Annual Dues - Instructor or Tow Pilot Members*</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$600.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Volunteer Instructor or Tow Pilot Members**</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$0.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Annual Dues - Social Member</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$75.00</asp:TableCell>
            </asp:TableRow>
            </asp:Table>
        <h4>Soaring Society of America Dues (required for PSSA membership)</h4>
        <asp:Table CssClass="rstable" runat="server" BorderWidth="2px">
            <asp:TableRow>
                <asp:TableCell>Yearly Dues - Regular Active Member</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$75.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Family Member</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$45.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Youth Member</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$42.00</asp:TableCell>
            </asp:TableRow>
            </asp:Table>
        <h4>Flight-Related Fees***</h4>
        <asp:Table CssClass="rstable" runat="server" BorderWidth="2px">
            <asp:TableRow>
                <asp:TableCell>Aerotow - First 1000' of altitude difference</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$32.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Aerotow - Each Additional 100'</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$1.40</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Aerotow - If tow altitude difference <= 500 feet, typically for simulated rope break</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$21.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Glider Rental per hour, prorated to the minute, minimum 20 minutes ($12.00)</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$36.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Glider retrieval by tow plane (tachometer hour rate, 1/10th hour increments) and any other tow plane uses.</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$160.00</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Minimum Monthly Flying Charges for Regular, Family, and Youth Members****</asp:TableCell>
                <asp:TableCell CssClass="rsrightalign">$72.00</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <p>* PSSA annual dues are payable in two equal instalments, in January and July. If the entire annual dues are paid before February 15, an 8.33% discount is applied.</p>
        <p>** Volunteer Instructors and Tow Pilots are members who only provide those specific services to PSSA and are not eligible for holding office,
            voting, or renting PSSA aircraft. Please refer to the <asp:HyperLink NavigateUrl="bylaws.pdf" runat="server">Bylaws</asp:HyperLink>
            and <asp:HyperLink NavigateUrl="operations.pdf" runat="server">Operating Rules</asp:HyperLink> for complete details.</p>
        <p>*** For billing, tow and rental amounts are rounded up to the nearest whole dollar.</p>
        <p>**** During the flying season (normally March through October), regular, family, and youth members are required to spend at least $72/month on flight-related fees.
            The requirement is cumulative, i.e., if a member spent below the minimum up to a particular month, the member can make up for it by spending above the minimum
            in subsequent months of the same flying season.</p>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
