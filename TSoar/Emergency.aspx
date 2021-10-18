<%@ Page Title="Emergency" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Emergency.aspx.cs" Inherits="TSoar.Emergency" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .emergency { font-size:x-large}
    </style>
    <div class="HelpText" ><%-- SCR 213 --%>
        <div class="emergency">
            <ul>
                <li>Dial 911</li>
                <li>Give Official Location</li>
                <ul>
                    <li>Puget Sound Soaring Association (PSSA)</li>
                    <li>Operates out of Bergseth Air Field:</li>
                    <ul>
                        <li>31500 S.E. 408th Street</li>
                        <li>Enumclaw, WA 98022</li>
                    </ul>
                    <li>Coordinates:</li>
                    <ul>
                        <li>Degrees-minutes-seconds:<br /> 47-14-36.8660 N, 121-55-28.3940 W</li>
                        <li>Degrees and fractions:<br /> 47.2435739, -121.9245539</li>
                    </ul>
                </ul>
                <li>Give PSSA Operations Phone Number<br />206 660 0019</li>
            </ul>
        </div>
        <p>The Field Manager is responsible for executing the Emergency Response Procedure, and will remain in charge until that
            responsibility is turned over to another club member by agreement.
        </p>
        <p>Operations may need to be shut down for the day.</p>
        <p>Fully cooperate with all emergency services personnel and the FAA - NTSB. (Investigation officials will ask questions; only the questions asked should be
            answered. The response should be based on first-hand knowledge. Do not respond with opinions, speculations, suppositions, or conclusions.)</p>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>