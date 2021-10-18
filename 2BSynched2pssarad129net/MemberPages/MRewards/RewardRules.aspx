<%@ Page Title="RewardRules" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="RewardRules.aspx.cs" Inherits="TSoar.MemberPages.MRewards.RewardRules" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <asp:Label runat="server" Text="The Rules for Earning and Claiming Service Points" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p><a href="MRewards.aspx">Show my Rewards History</a></p>
        <h3>Rules for Earning and Claiming Service Points</h3>
        <ol type="1">
            <li>Tow pilots and instructors accumulate ‘Service Points’, regardless of club membership category.</li>
            <li>Three servicepoints are earned for each flight flown as instructor, or as tow pilot when pilot in command.</li>
            <li>Additional service points are earned:</li>
            <ol type="a">
                <li>Single signup:</li>
                <ol type="i">
                    <li>Ten service points when a tow pilot signs up on SignUpGenius for towing for a particular day and then shows up for duty, or </li>
                    <li>Ten service points when an instructor signs up on SignUpGenius for instructing for a particular day and then shows up for duty; </li>
                </ol>
                <li>Multiple signup: Up to two tow pilots and two instructors may sign up on SignUpGenius. When those two tow pilots or those two instructors show up for duty,
                        they earn five service points each. </li>
                <li>No additional service points are earned for showing up without signing up first and towing or instructing. No service points are earned for non-towing flights.</li>
                <li>When an individual signs up for both instructor and towing duty on the same day only ten points will be awarded, or if duty is shared with another instructor or tow pilot a maximum of five service points is earned.</li>
                <li>When a tow pilot or instructor agrees to provide services for an event not scheduled via SignUpGenius, that pilot is deemed to have signed up for purposes of awarding additional service points; the bookkeeper needs to be notified so that the additional service points can be recorded. </li>
            </ol>
            <li>The accumulation of earned service points is automatic as the bookkeeper will credit service points from the daily flight logs and SignUpGenius and maintain a record of member balances.</li>
            <li>Service points have value of one service point for one dollar, only within PSSA. </li>
            <li>Service points are intended to be used to cover the claimant’s own glider rental and glider tow charges. They may not be used to pay for other charges, such as assessments and membership dues.</li>
            <li>Tow pilots and instructor service points can be used to pay for tows and glider rental charges of other members with mutual agreement of both parties (see item 11.b below).</li>
            <li>Service points can be used for purchasing gift certificate temporary memberships for non-members, assuming the tow pilot/instructor account contains a minimum balance of service points to purchase the gift certificate (see Item 11.c below).</li>
            <li>Service points expire at the end of the soaring season, except that service points earned during the last two months of one season carry over to the next season. (The soaring season usually lasts from March 1 to October 31.)</li>
            <li>For any one instructor or tow pilot member, the total number of service points claimed through rewards during one calendar year must not exceed 600.</li>
            <li>Procedures for claiming service points:</li>
            <ol type="a">
                <li>When a tow pilot or instructor desires to use service points for a tow and/or glider rental he/she should notify the field manager at the time of the operation. The field manager will note the intent in the appropriate place on the daily log.   The bookkeeper will apply the claim as part of his/her normal billing duties.</li>
                <li>When an instructor or tow pilot intends to pay tow fees and glider rental charges of another member this transaction is to be managed AFTER THIS FACT by notifying the bookkeeper via email of the intent to pay charges of the member.   The bookkeeper will request the receiving member to acknowledge and approve of the request.     With email acknowledgement and approval of both parities, the bookkeeper will apply the service point claim to the operation. Service points may be gifted to another club member up to 300 service point per soaring season from any one instructor or tow pilot.</li>
                <li>To purchase a gift certificate for temporary membership with service points, the instructor/tow pilot must initiate the certificate purchase IN ADVANCE so that the transaction is completed prior to having the guest come to the field.  This should be accomplished by notifying the bookkeeper ahead of the date of the intent to purchase the certificate, and the bookkeeper will then apply service points and award the certificate.</li>
            </ol>
            <li>The Bookkeeper is charged with keeping records of service points earned, claimed, and expired, and making them visible to tow pilots and instructors in a timely manner.</li>
            <li>The Treasurer (in conjunction with the bookkeeper) is charged with applying the service point rewards to tow pilots’ and instructors’ accounts. Service point accounts are not allowed to go negative (no ‘borrowing’). Service point accounts are strictly separate from real money accounts. Service point accounts do not show up in the Club’s bookkeeping of real money.</li>
            <li>These rules apply starting on June 1, 2019. </li>
        </ol>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
