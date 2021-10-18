<%@ Page Title="Field Manager Support" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="FieldMgr.aspx.cs" Inherits="TSoar.MemberPages.FieldMgr" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Information for Field Managers" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h4>Field Manager Manual</h4>
        <p>As of September 2021, a comprehensive manual for the Field Manager is available:</p>
        <p><a href="https://1drv.ms/b/s!AvoFL8QrGVaTgZtV84B1zeeD4sPz1Q?e=ojrJmg" target="_blank">Open Field Manager Manual (pdf document)</a></p>
    </div>
<%--    <ajaxToolkit:Accordion
        ID="AccordionFldMgr"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccP_Intro" runat="server" >
                <Header>Introduction</Header>
                <Content>
                    <div class="HelpText" >
                        <h4>Field Manager Manual</h4>
                        <p>As of September 2021, a comprehensive manual for the field manager is available:</p>
                        <p><a href="https://1drv.ms/b/s!AvoFL8QrGVaTgZtV84B1zeeD4sPz1Q?e=ojrJmg" target="_blank">Open Field Manager Manual (pdf document)</a></p>
                        <h4>The Field Manager's Authority</h4>
                        <p>The Field Manager's authority is delegated directly from the Board of Directors.
                        The Field Manager’s authority and decisions are to be respected and accepted by all.
                        However, anyone may question decisions concerning safety of flight, and the most conservative action shall prevail.</p>
                        <h4>The Field Manager's Principal Responsibilities</h4>
                        <ol>
                            <li>Maintain a safe environment for club members and equipment</li>
                            <li>Maintain an orderly operation</li>
                            <li>Maintain a record of operations</li>
                            <li>Represent the club in the absence of board members, to the general public and to the property owners</li>
                            <li>Be familiar with the PSSA Operations Rules and Procedures</li>
                            <li>Use the checklist below as an aid in fulfilling these responsibilities. This checklist also appears on the reverse side of daily log sheets.</li>
                        </ol>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccP_Instr" runat="server" >
                <Header> Checklist </Header>
                <Content>
                    <div class="HelpText" >
                        <h4>Planning</h4>
                            <ul>
                                <li>Weather conditions checked</li>
                                <li>Contacted FSS for NOTAM information (800-992-7433)</li>
                                <li>Towpilot and instructor coordinated</li>
                                <li>Operations message recorded</li>
                            </ul>
                        <h4>Preflight</h4>
                            <ul>
                                <li>Road signs set up</li>
                                <li>Field/runway checked (any rocks larger than half a fist?)</li>
                                <li>Field cellphone "on" and at hand and monitored</li>
                                <li>Operations table set up (logbooks, sign-up sheets/daily log, tow cards, radios)</li>
                                <li>Gliders ready for flight (untied, preflighted, & logbooks signed)</li>
                                <li>Towrope & connecting link inspected and towrope attached to tow plane</li>
                                <li>Briefings and assignments made</li>
                            </ul>
                        <h4>Flight Operation</h4>
                            <ul>
                                <li>Towplane staging area cleared</li>
                                <li>Runway clearance maintained (cars, trailers, gliders, people)</li>
                                <li>Glider flights safely expedited</li>
                                <li>Situation awareness & order maintained</li>
                                <li>Daily operating information recorded in daily log</li>
                                <li>Give bottom portion of green introductory membership application to the applicant as a receipt</li>
                                <li>Aerobatic flight is prohibited around Bergseth Field. At other locations, make a note in the log.</li>
                            </ul>
                        <h4>Post-flight</h4>
                            <ul>
                                <li>Gliders parked and tied down</li>
                                <li>Batteries removed & on charger</li>
                                <li>Cellphone on charger</li>
                                <li>Handheld radios on charger – see instructions on back wall of charging station (!)</li>
                                <li>Towrope & connecting link put away</li>
                                <li>Power mower in shed</li>
                                <li>All equipment put away</li>
                                <li>Area cleaned up</li>
                                <li>Paperwork:</li>
                                    <ul>
                                        <li>Yellow Liability Release forms – in 3-ring binder stored in shed</li>
                                        <li>Top portion of green temporary membership forms – send with daily log</li>
                                        <li>Blue membership forms – send with daily log</li>
                                        <li>Redeemed gift certificates - send with daily log</li>
                                        <li>Do not send cash – substitute your own personal check</li>
                                        <li>Send to bookkeeper (use provided envelope) or club mailbox at PSSA, Box 941, Enumclaw, WA 98022</li>
                                    </ul>
                                <li>Training center/shed, and charging station box locked</li>
                                <li>Rope fence up at end of runway (when available)</li>
                                <li>Last PSSA person of the day closes gate (road entrance)</li>
                                <li>Road signs stored</li>
                                <li>Notify the club Maintenance officer of any equipment issues requiring attention.
                                    If a glider has been found to be in an unairworthy condition, make an entry in the pre-flight log and place a notice in the glider cockpit</li>
                                <li>After the day's operation, the field manager should contact the scheduled field manager for the next operating day
                                    and provide summary status and condition of all club aircraft, equipment (like radios, batteries, towropes, etc), and field.
                                    Providing this information ahead of time will help field managers better plan the day's operation.</li>
                            </ul>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane ID="AccP_Telephone" runat="server" >
                <Header> Telephone </Header>
                <Content>
                    <div class="HelpText" >
                        <p>The Club maintains a cellular field telephone. When not used by the Field Manager it is kept in the Club's storage structure connected to its charging device.
                        The number is 206 660 0019. Service is provided by AT&T.</p>
                        <p>When members of the public call this number and nobody answers, there should be an appropriate message. Here is suggested wording:</p>
                        <p class="para-indent2"><i>This is Puget Sound Soaring Association. We are a glider flying club operating mostly on weekend days from the beginning of March to the end of October
                            from Bergseth Field NorthEast of Enumclaw, Washington. Find us on the Web at Pugetsoundsoaring.org.
                            Check this line after 9:30 am on days of operation for a message about plans for the day.
                            You may leave us a message after the beep.</i></p>
                        <p>Suggested wording for the operations message to be recorded as the 'temporary absence greeting' before 9:30am on a day of glider operations under favorable conditions:</p>
                        <p class="para-indent2"><i>This is Puget Sound Soaring Association operations message for Saturday, September 7, 2019. Today's weather forecast looks great; 
                            many pilots have signed up; all equipment is in top shape. Operations will start at 10 am.
                            Our instructor will be Chesley Sullenberger, and our tow pilot will be Amelia Earhart.
                            I am Charles Lindberg, your field manager, looking forward to seeing many of you at Bergseth Field today.</i>
                        </p>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>--%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
