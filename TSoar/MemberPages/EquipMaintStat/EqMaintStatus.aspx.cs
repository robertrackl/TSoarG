using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using TSoar.DB;
using TSoar.Equipment;

namespace TSoar.MemberPages.EquipMaintStat
{
    public partial class EqMaintStatus : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            gvEqMSt.PageSize = int.Parse(mCRUD.GetSetting("PageSizeEquipMaintStatus"));
        }

        protected void gvEqMSt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    System.Drawing.Color foreColor;
                    switch ((byte)DataBinder.Eval(e.Row.DataItem, "% Complete"))
                    {
                        case 0:
                            if (String.Compare((string)DataBinder.Eval(e.Row.DataItem, "Deadline"), 
                                CustFmt.sFmtDate(DateTimeOffset.Now.Subtract(new TimeSpan(7, 0, 0, 0)),
                                CustFmt.enDFmt.DateOnly),StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                foreColor = System.Drawing.Color.FromArgb(200, 38, 0);
                            }
                            else
                            {
                                foreColor = System.Drawing.Color.OrangeRed;
                            }
                            break;
                        case 100:
                            foreColor = System.Drawing.Color.Blue;
                            break;
                        default:
                            foreColor = System.Drawing.Color.Black;
                            break;
                    }
                    e.Row.ForeColor = foreColor;
                    break;
            }
        }

        protected void gvEqMSt_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEqMSt.PageIndex = e.NewPageIndex;
        }

        protected void gvEqMSt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iRow = 0;
            if (Int32.TryParse((string)e.CommandArgument, out iRow))
            {
                GridViewRow row = gvEqMSt.Rows[iRow];
                switch (row.RowType)
                {
                    case DataControlRowType.DataRow:
                        Label lblIID = (Label)row.FindControl("lblIID");
                        vDetails(lblIID.Text);
                        break;
                }
            }
        }

        private void vDetails(string suActionItemID)
        {
            int iActionItem = int.Parse(suActionItemID);
            Equipment.EquipmentDataContext eqdc = new Equipment.EquipmentDataContext();
            int iAgingItem = (from a in eqdc.EQUIPACTIONITEMs where a.ID == iActionItem select a.iEquipAgingItem).First();
            var qAgingItem = (from w in eqdc.EQUIPAGINGITEMs
                              where w.ID == iAgingItem
                              select w).First();
            var qEquipCompon = (from c in eqdc.EQUIPCOMPONENTs
                                where c.ID == qAgingItem.iEquipComponent
                                select c).First();
            var qPoEq = (from e in eqdc.EQUIPMENTs
                         where e.ID == qEquipCompon.iEquipment
                         select new
                         {
                             e.sShortEquipName,
                             e.DOwnershipBegin,
                             e.DOwnershipEnd,
                             e.EQUIPTYPE.sEquipmentType
                         }).First();
            var qParSet = (from p in eqdc.EQUIPAGINGPARs
                           where p.ID == qAgingItem.iParam
                           select new { p.EQUIPACTIONTYPE.sEquipActionType, p }).First();
            var qOpsCal = (from o in eqdc.OPSCALNAMEs
                           where o.ID == qAgingItem.iOpCal
                           select new
                           {
                               o.sOpsCalName,
                               iOpsCal = o.ID
                           }).First();
            var qOpsCalTimes = (from m in eqdc.OPSCALTIMEs where m.iOpsCal == qOpsCal.iOpsCal select m).ToList();
            var qOperDat = (from d in eqdc.EQUIPOPERDATAs
                            where d.iEquipComponent == qAgingItem.iEquipComponent
                            orderby d.DFrom
                            select d).ToList();
            var qFlyOpsDat = (from f in eqdc.OPDETAILs
                              where f.iEquip == qEquipCompon.iEquipment
                              orderby f.OPERATION.DBegin
                              select f).ToList();
            lblActItemID.Text = suActionItemID;
            lblEquipName.Text = qPoEq.sShortEquipName;
            lblEquipFrom.Text = qPoEq.DOwnershipBegin.ToString();
            lblEquipTo.Text = qPoEq.DOwnershipEnd.ToString();
            lblEquipProps.Text = qPoEq.sEquipmentType;
            lblComponName.Text = Equipment.EqSupport.sExpandedComponentName(qAgingItem.iEquipComponent);
            lblComponFrom.Text = qEquipCompon.DLinkBegin.ToString();
            lblComponTo.Text = qEquipCompon.DLinkEnd.ToString();
            lblComponProps.Text = "bEntire = " + qEquipCompon.bEntire.ToString();
            lblAgingItemName.Text = qAgingItem.sName;
            lblAgingItemFrom.Text = qAgingItem.DStart.ToString();
            lblAgingItemTo.Text = qAgingItem.DEnd.ToString();
            lblAgingItemProps.Text = qAgingItem.sComment;
            lblAgingItemProps2.Text = "dEstDuration = " + qAgingItem.dEstDuration.ToString() + " days";
            lblParSetName.Text = qParSet.p.sShortDescript;
            lblParSetProps.Text = qParSet.sEquipActionType;
            if (qParSet.p.iIntervalElapsed > -1)
            {
                lblParSetProps.Text += ", iIntervalElapsed = " + qParSet.p.iIntervalElapsed.ToString() + " " + qParSet.p.sTimeUnitsElapsed +
                    ", cDeadLineMode = " + qParSet.p.cDeadLineMode + ", iDeadLineSpt1 = " + qParSet.p.iDeadLineSpt1.ToString() +
                    ", iDeadLineSpt2 = " + qParSet.p.iDeadLineSpt2.ToString();
            }
            if (qParSet.p.iIntervalOperating > -1)
            {
                lblParSetProps.Text += ", iIntervalOperating = " + qParSet.p.iIntervalOperating.ToString() + " " + qParSet.p.sTimeUnitsOperating;
                lblAgingItemProps2.Text += ", dEstRunDays = " + qAgingItem.dEstRunDays.ToString() + ", bRunExtrap = " + qAgingItem.bRunExtrap.ToString();
            }
            if (qParSet.p.iIntervalCycles > -1)
            {
                lblParSetProps.Text += ", iIntervalCycles = " + qParSet.p.iIntervalCycles.ToString();
                lblAgingItemProps2.Text += ", dEstCycleDays = " + qAgingItem.dEstCycleDays.ToString() + ", bCyclExtrap = " + qAgingItem.bCyclExtrap.ToString();
            }
            if (qParSet.p.iIntervalDistance > -1)
            {
                lblParSetProps.Text += ", iIntervalDistance = " + qParSet.p.iIntervalDistance.ToString() + " " + qParSet.p.sDistanceUnits;
                lblAgingItemProps2.Text += ", dEstDistDays = " + qAgingItem.dEstDistDays.ToString() + ", bDistExtrap = " + qAgingItem.bDistExtrap.ToString();
            }
            if (qParSet.p.iIntervalElapsed == -1 && qParSet.p.iIntervalOperating == -1 && qParSet.p.iIntervalCycles == -1 && qParSet.p.iIntervalDistance == -1)
            {
                lblAgingItemProps2.Text += ", unique event";
            }
            if (!(qParSet.p.sComment is null))
            {
                if (qParSet.p.sComment.Length > 0) lblParSetProps.Text += ", " + qParSet.p.sComment;
            }
            lblOpsCalName.Text = qOpsCal.sOpsCalName;
            lblOpsCalFrom.Text = qOpsCalTimes.First().DStart.ToString();
            lblOpsCalTo.Text = qOpsCalTimes.Last().DStart.ToString();
            lblOpsCalProps.Text = "Number of segments = " + qOpsCalTimes.Count.ToString();
            if (qOperDat.Count > 0)
            {
                lblManOpsFrom.Text = qOperDat.First().DFrom.ToString();
                lblManOpsTo.Text = qOperDat.Last().DTo.ToString();
                lblManOpsProps.Text = qOperDat.Count.ToString() + " data points";
            }
            else { lblManOpsFrom.Text = ""; lblManOpsTo.Text = ""; lblManOpsProps.Text = ""; }
            if (qFlyOpsDat.Count > 0)
            {
                lblFlyOpsFrom.Text = qFlyOpsDat.First().OPERATION.DBegin.ToString();
                lblFlyOpsTo.Text = qFlyOpsDat.Last().OPERATION.DBegin.ToString();
                lblFlyOpsProps.Text = qFlyOpsDat.Count.ToString() + " flights";
            }
            else { lblFlyOpsProps.Text = ""; lblFlyOpsFrom.Text = ""; lblFlyOpsTo.Text = ""; }

            var qAct = (from c in eqdc.EQUIPACTIONITEMs where c.ID == iActionItem select c).First();
            lblPiTRecordEntered.Text = qAct.PiTRecordEntered.ToString();
            lblRecordEnteredBy.Text = (from p in eqdc.PEOPLEs where p.ID == qAct.iRecordEnteredBy select p.sDisplayName).First();
            lblDeadLine.Text = qAct.DeadLine.ToString();
            lblDeadlineHrs.Text = qAct.dDeadlineHrs.ToString() + " hours";
            lblDeadLineCycles.Text = qAct.iDeadLineCycles.ToString() + " cycles";
            lblDeadLineDist.Text = qAct.dDeadLineDist.ToString() + " " + qParSet.p.sDistanceUnits;
            lblScheduledStart.Text = qAct.DScheduledStart.ToString();
            lblActualStart.Text = qAct.DActualStart.ToString();
            lblPercentComplete.Text = qAct.iPercentComplete.ToString();
            lblAtCompletionHrs.Text = qAct.dAtCompletionHrs.ToString();
            lblAtCompletionCycles.Text = qAct.iAtCompletionCycles.ToString();
            lblAtCompletionDist.Text = qAct.dAtCompletionDist.ToString();
            lblDComplete.Text = qAct.DComplete.ToString();
            lblComment.Text = qAct.sComment;
            lblUpdateStatus.Text = qAct.sUpdateStatus;

            int iEqActionItem = int.Parse(suActionItemID);
            tbpIllustr.Visible = false;
            tbpChartsH.Visible = false;
            tbpChartsC.Visible = false;
            tbpChartsD.Visible = false;

            foreach (EqSupport.enSchedMeth s in Enum.GetValues(typeof(EqSupport.enSchedMeth)))
            {
                switch (s)
                {
                    case EqSupport.enSchedMeth.Illustration:
                        var qh = from h in eqdc.EQACITAUXes
                                    where h.iEqActionItem == iEqActionItem && h.cKind == 'I'
                                    select h;
                        foreach (var h in qh)
                        {
                            if (h.sIllustrFile.Length > 0)
                            {
                                imgIllustr.ImageUrl = h.sIllustrFile;
                                tbpIllustr.Visible = true;
                            }
                        }
                        break;
                    case EqSupport.enSchedMeth.ElapsedTime:
                        break;
                    case EqSupport.enSchedMeth.OperatingHours:
                        qh = from h in eqdc.EQACITAUXes
                                where h.iEqActionItem == iEqActionItem && h.cKind == 'H'
                                select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsH.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapH.Serializer.Content = SerializationContents.Default;
                            chartExtrapH.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapH.Width = 900;
                            chartExtrapH.Height = 660;
                            tbpChartsH.Visible = true;
                        }
                        break;
                    case EqSupport.enSchedMeth.Cycles:
                        qh = from h in eqdc.EQACITAUXes
                                where h.iEqActionItem == iEqActionItem && h.cKind == 'C'
                                select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsC.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapC.Serializer.Content = SerializationContents.Default;
                            chartExtrapC.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapC.Width = 900;
                            chartExtrapC.Height = 660;
                            tbpChartsC.Visible = true;
                        }
                        break;
                    case EqSupport.enSchedMeth.DistanceTraveled:
                        qh = from h in eqdc.EQACITAUXes
                                where h.iEqActionItem == iEqActionItem && h.cKind == 'D'
                                select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsD.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapD.Serializer.Content = SerializationContents.Default;
                            chartExtrapD.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapD.Width = 900;
                            chartExtrapD.Height = 660;
                            tbpChartsD.Visible = true;
                        }
                        break;
                }
            }


            ModPopExtDetails.Show();
        }

    }
}