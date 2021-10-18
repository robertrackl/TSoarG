using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Statistician
{
    public partial class OpsDataInput : System.Web.UI.Page
    {
        #region Declarations
        private const string scDateTimeFmt = "yyyy-MM-ddTHH:mm:ss";
        private const string scEdUpCanceled = "; attempted add/edit/update was canceled.";
        private static readonly string[] saMonths = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private enum enumLast { LastTakeoff, LastLanding,
            LastTakeoffHour, LastTakeoffMinute,
            LastLandingHour, LastLandingMinute };
        private enum enumAddEdit { None, AddAviator, EditAviator, AddSpecialOp, EditSpecialOp, AddOpDetail, EditOpDetail, AddOperation, EditOperation};
        private static DateTime Dold = new DateTime(2001, 1, 2); // is like a constant (never assigned to except here)
        private int[] iaLast = new int[] { 0, 0, -1, -1, -1, -1 }; // First two array elements intentionally never used
        private string[] saLastLoc = new string[] { "", "" };
        private string sLastLaunchMethod = "";
        private DateTime[] DaLast = new DateTime[] { new DateTime(2001, 1, 1), new DateTime(2001, 1, 1) };
        private string[] sa;
        private TreeNode tnOp;
        private TreeNode tnOD;
        private SCUD_Multi mCRUD = new SCUD_Multi();
        private SCUD_single sCRUD = new SCUD_single();

        private enumAddEdit eAddEdit { get { return (enumAddEdit?)ViewState["eAddEdit"] ?? enumAddEdit.None; } set { ViewState["eAddEdit"] = value; } }
        private int iID_Op { get { return (int?)ViewState["iID_Op"] ?? 0; } set { ViewState["iID_Op"] = value; } }
        private int iID_SpOp { get { return (int?)ViewState["iID_SpOp"] ?? 0; } set { ViewState["iID_SpOp"] = value; } }
        private int iID_OD { get { return (int?)ViewState["iID_OD"] ?? 0; } set { ViewState["iID_OD"] = value; } }
        private int iID_Av { get { return (int?)ViewState["iID_Av"] ?? 0; } set { ViewState["iID_Av"] = value; } }

        private PopulateTrV PopulTree = new PopulateTrV();
        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["iaLast"] != null)
            {
                iaLast = Session["iaLast"] as int[];
            }
            if (Session["sLastLaunchMethod"] != null)
            {
                sLastLaunchMethod = Session["sLastLaunchMethod"] as string;
            }
            if (Session["saLastLoc"] != null)
            {
                saLastLoc = Session["saLastLoc"] as string[];
            }
            if (Session["DaLast"] != null)
            {
                DaLast = Session["DaLast"] as DateTime[];
            }
            if (sLastLaunchMethod.Length < 1)
            {
                sLastLaunchMethod = mCRUD.GetSetting("DefaultLaunchMethod");
                Session["sLastLaunchMethod"] = sLastLaunchMethod;
            }
            if (saLastLoc[(int)enumLast.LastTakeoff].Length < 1)
            {
                saLastLoc[(int)enumLast.LastTakeoff] = mCRUD.GetSetting("DefaultLocation");
            }
            if (DaLast[(int)enumLast.LastTakeoff] < Dold)
            {
                DaLast[(int)enumLast.LastTakeoff] = (DateTime.Today).AddDays(-1.0);
            }
            if ( iaLast[(int)enumLast.LastTakeoffHour] < 0)
            {
                iaLast[(int)enumLast.LastTakeoffHour] = 11;
            }
            if (iaLast[(int)enumLast.LastTakeoffMinute] < 0)
            {
                iaLast[(int)enumLast.LastTakeoffMinute] = 0;
            }
            if (saLastLoc[(int)enumLast.LastLanding].Length < 1)
            {
                saLastLoc[(int)enumLast.LastLanding] = saLastLoc[(int)enumLast.LastTakeoff];
            }
            if (DaLast[(int)enumLast.LastLanding] < Dold)
            {
                DaLast[(int)enumLast.LastLanding] = (DateTime.Today).AddDays(-1.0);
            }
            if (iaLast[(int)enumLast.LastLandingHour] < 0)
            {
                iaLast[(int)enumLast.LastLandingHour] = 12;
            }
            if (iaLast[(int)enumLast.LastLandingMinute] < 0)
            {
                iaLast[(int)enumLast.LastLandingMinute] = 0;
            }
            Session["iaLast"] = iaLast;
            Session["saLastLoc"] = saLastLoc;
            Session["DaLast"] = DaLast;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ButtonsStyle(trv_Ops.Nodes[0]);
                PopulTree.trv_OpsClosestInTime(trv_Ops);
            }
        }

        #region Modal Popup
        //======================
        private void ButtonsClear()
        {
            NoButton.CommandArgument = "";
            NoButton.CommandName = "";
            YesButton.CommandArgument = "";
            YesButton.CommandName = "";
            OkButton.CommandArgument = "";
            OkButton.CommandName = "";
            CancelButton.CommandArgument = "";
            CancelButton.CommandName = "";
        }
        private void MPE_Show(Global.enumButtons eubtns)
        {
            NoButton.CssClass = "displayNone";
            YesButton.CssClass = "displayNone";
            OkButton.CssClass = "displayNone";
            CancelButton.CssClass = "displayNone";
            switch (eubtns)
            {
                case Global.enumButtons.NoYes:
                    NoButton.CssClass = "displayUnset";
                    YesButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkOnly:
                    OkButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkCancel:
                    OkButton.CssClass = "displayUnset";
                    CancelButton.CssClass = "displayUnset";
                    break;
            }
            ModalPopExt.Show();
        }
        protected void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "Operation":
                            // Delete the Operation
                            mCRUD.DeleteOne(Global.enugInfoType.Operations, btn.CommandArgument);
                            trv_Ops.SelectedNode.ToolTip = trv_Ops.SelectedNode.Text;
                            trv_Ops.SelectedNode.Text = "DELETED";
                            trv_Ops.Nodes.Remove(trv_Ops.SelectedNode);
                            break;
                    }
                }
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            ActivityLog.oLog(ActivityLog.enumLogTypes.PopupException, 0, lblPopupText.Text);
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        #region TreeView trv_Ops
        protected void trv_Ops_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeView trv = (TreeView)sender;
            ButtonsStyle(trv.SelectedNode);
            switch (trv.SelectedNode.Depth)
            {
                case (int)Global.enumDepths.Operation:
                    tnOp = trv.SelectedNode;
                    iID_Op = Int32.Parse(tnOp.Value);
                    break;
                case (int)Global.enumDepths.OpDetail:
                    tnOD = trv.SelectedNode;
                    iID_OD = Int32.Parse(tnOD.Value);
                    tnOp = tnOD.Parent;
                    iID_Op = Int32.Parse(tnOp.Value);
                    break;
                case (int)Global.enumDepths.Aviator:
                    tnOD = trv.SelectedNode.Parent;
                    iID_OD = Int32.Parse(tnOD.Value);
                    tnOp = tnOD.Parent;
                    iID_Op = Int32.Parse(tnOp.Value);
                    break;
            }
        }
        protected void trv_Ops_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            if (e.Node.ChildNodes.Count == 0)
            {
                switch (e.Node.Depth)
                {
                    case (int)Global.enumDepths.Root:
                        PopulTree.PopulateYears(e.Node);
                        break;
                    case (int)Global.enumDepths.Year:
                        PopulTree.PopulateMonths(e.Node);
                        break;
                    case (int)Global.enumDepths.Month:
                        PopulTree.PopulateDays(e.Node);
                        break;
                    case (int)Global.enumDepths.Day:
                        PopulTree.PopulateOperations(e.Node, true);
                        break;
                    case (int)Global.enumDepths.Operation:
                        PopulTree.PopulateSpecialOpsInfo(e.Node);
                        PopulTree.PopulateOpDetails(e.Node);
                        break;
                    case (int)Global.enumDepths.OpDetail:
                        PopulTree.PopulateAviators(e.Node);
                        break;
                }
            }
        }

        protected void trv_Ops_PreRender(object sender, EventArgs e)
        {
            foreach (TreeNode t0 in trv_Ops.Nodes) // root
            {
                foreach (TreeNode t1 in t0.ChildNodes) // years
                {
                    foreach (TreeNode t2 in t1.ChildNodes) // months
                    {
                        foreach (TreeNode t3 in t2.ChildNodes) // days
                        { 
                            foreach (TreeNode t4 in t3.ChildNodes) // Operations
                            {
                                ValidateOperation(t4);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Add New / Edit Operation *******************
        protected void pbAddOp_Click(object sender, EventArgs e)
        {
            lblOpTitle.Text = "Add Operation";
            eAddEdit = enumAddEdit.AddOperation;
            pbOpDelete.CssClass = "displayNone";
            ModalPopExt4Op.Show();
        }

        protected void pbEditOp_Click(object sender, EventArgs e)
        {
            if (!((trv_Ops.SelectedNode.Text.IndexOf("DELETED") > -1) && (trv_Ops.SelectedNode.Text.Length == 7)))
            {
                lblOpTitle.Text = "Edit Operation";
                eAddEdit = enumAddEdit.EditOperation;
                pbOpDelete.CssClass = "displayUnset";
                ModalPopExt4Op.Show();
            }
        }

        #region Op (Operation) Popup ====================
        #region PreRender Event Handlers
        protected void DDL_LaunchMethod_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = sLastLaunchMethod;
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT LAUNCHMETHODS.sLaunchMethod " +
                            "FROM LAUNCHMETHODS INNER JOIN OPERATIONS ON LAUNCHMETHODS.ID = OPERATIONS.iLaunchMethod " +
                            "WHERE(OPERATIONS.ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }
        protected void DDL_TLoc_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = saLastLoc[(int)enumLast.LastTakeoff];
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT LOCATIONS.sLocation " +
                            "FROM OPERATIONS INNER JOIN LOCATIONS ON OPERATIONS.iTakeoffLoc = LOCATIONS.ID " +
                            "WHERE(OPERATIONS.ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }
        protected void DDL_LLoc_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = saLastLoc[(int)enumLast.LastLanding];
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT LOCATIONS.sLocation " +
                            "FROM OPERATIONS INNER JOIN LOCATIONS ON OPERATIONS.iLandingLoc = LOCATIONS.ID " +
                            "WHERE(OPERATIONS.ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }
        protected void txbTDate_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = DaLast[(int)enumLast.LastTakeoff].ToString("yyyy-MM-dd");
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT CONVERT(varchar, DBegin, 111) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = ((string)ob).Replace("/", "-"); }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void txbLDate_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = DaLast[(int)enumLast.LastLanding].ToString("yyyy-MM-dd");
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT CONVERT(varchar, DEnd, 111) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = ((string)ob).Replace("/", "-"); }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void txbTTHour_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    iaLast = Session["iaLast"] as int[];
                    ss = iaLast[(int)enumLast.LastTakeoffHour].ToString();
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT DATENAME(hour, DBegin) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = (string)ob; }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void txbTTMinute_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    iaLast = Session["iaLast"] as int[];
                    ss = iaLast[(int)enumLast.LastTakeoffMinute].ToString();
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT DATENAME(minute, DBegin) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = (string)ob; }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void txbTLHour_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    iaLast = Session["iaLast"] as int[];
                    ss = iaLast[(int)enumLast.LastLandingHour].ToString();
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT DATENAME(hour, DEnd) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = (string)ob; }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void txbTLMinute_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    iaLast = Session["iaLast"] as int[];
                    ss = iaLast[(int)enumLast.LastLandingMinute].ToString();
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT DATENAME(minute, DEnd) " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = (string)ob; }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        protected void DDL_ChargeCode_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = "Full";
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT CHARGECODES.sChargeCode " +
                            "FROM CHARGECODES INNER JOIN OPERATIONS ON CHARGECODES.ID = OPERATIONS.iChargeCode " +
                            "WHERE(OPERATIONS.ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }
        protected void txb_Notes_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = "";
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT sComment " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object ob = cmd.ExecuteScalar();
                            if (ob == DBNull.Value) { ss = ""; } else { ss = (string)ob; }
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }

        protected void txbInvoices2go_PreRender(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    ss = "-1"; // The number of invoices still to be generated from this operation is to be determined later
                    break;
                case enumAddEdit.EditOperation:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT iInvoices2go " +
                            "FROM OPERATIONS " +
                            "WHERE(ID = " + ss + ")"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            int iInvoices2go = (int)cmd.ExecuteScalar();
                            ss = iInvoices2go.ToString();
                        }
                    }
                    break;
                default:
                    return;
            }
            tb.Text = ss;
        }
        #endregion PreRender

        protected void pbOpOK_Click(object sender, EventArgs e)
        {
            string sNodeText = "";
            //sa = new string[10]; // SCR 217
            sa = new string[11]; // SCR 217
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[2] = ((DropDownList)dv_Op.FindControl("DDL_LaunchMethod")).SelectedValue.ToString();
            sLastLaunchMethod = ((DropDownList)dv_Op.FindControl("DDL_LaunchMethod")).SelectedItem.Text;
            Session["sLastLaunchMethod"] = sLastLaunchMethod;
            sNodeText = sLastLaunchMethod;
            // Takeoff
            sa[3] = ((DropDownList)dv_Op.FindControl("DDL_TLoc")).SelectedValue.ToString();
            saLastLoc[(int)enumLast.LastTakeoff] = ((DropDownList)dv_Op.FindControl("DDL_TLoc")).SelectedItem.Text;
            sNodeText += " T " + saLastLoc[(int)enumLast.LastTakeoff];
            DateTime dT = Convert.ToDateTime(((TextBox)dv_Op.FindControl("txbTDate")).Text);
            int iTh = Int32.Parse(((TextBox)dv_Op.FindControl("txbTHour")).Text);
            int iTm = Int32.Parse(((TextBox)dv_Op.FindControl("txbTMinute")).Text);
            dT = dT.AddMinutes(60 * iTh + iTm);
            sa[4] = dT.ToString(scDateTimeFmt);
            sNodeText += " " + dT.ToString("yyyy/MM/dd HH:mm");
            // Landing
            sa[5] = ((DropDownList)dv_Op.FindControl("DDL_LLoc")).SelectedValue.ToString();
            saLastLoc[(int)enumLast.LastLanding] = ((DropDownList)dv_Op.FindControl("DDL_LLoc")).SelectedItem.Text;
            sNodeText += ", L " + saLastLoc[(int)enumLast.LastLanding];
            DateTime dL = Convert.ToDateTime(((TextBox)dv_Op.FindControl("txbLDate")).Text);
            int iLh = Int32.Parse(((TextBox)dv_Op.FindControl("txbLHour")).Text);
            int iLm = Int32.Parse(((TextBox)dv_Op.FindControl("txbLMinute")).Text);
            dL = dL.AddMinutes(60 * iLh + iLm);
            sa[6] = dL.ToString(scDateTimeFmt);
            sNodeText += " " + dL.ToString("yyyy/MM/dd HH:mm");
            // Notes
            sa[7] = Server.HtmlEncode(((TextBox)dv_Op.FindControl("txb_Notes")).Text);
            sa[8] = ((DropDownList)dv_Op.FindControl("DDL_ChargeCode")).SelectedValue.ToString();
            sNodeText += ", " + sa[7] + ", CC " + ((DropDownList)dv_Op.FindControl("DDL_ChargeCode")).SelectedItem.Text;
            sa[9] = ((TextBox)dv_Op.FindControl("txbInvoices2go")).Text;
            sNodeText += ", " + sa[9];
            sa[10] = "10"; // SCR 217
            //Validation
            if (dL <= dT)
            {
                lblPopupText.Text = "Takeoff Time " + sa[4] + " is not before landing time " + sa[6] + scEdUpCanceled;
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            switch (eAddEdit)
            {
                case enumAddEdit.AddOperation:
                    int i_ID = 0;
                    mCRUD.InsertOne(Global.enugInfoType.Operations, sa, out i_ID);
                    iID_Op = i_ID;
                    sNodeText = "Op " + iID_Op.ToString() + ": " + sNodeText;
                    trv_Ops.CollapseAll();
                    trv_Ops.Nodes[0].Expand();
                    // Expand to the just added operation
                    string ss = dT.Year.ToString();
                    string sExpandTo = "|" + ss;
                    TreeNode tn = trv_Ops.FindNode(sExpandTo);
                    if (tn == null)
                    {
                        tn = new TreeNode("y " + ss, ss);
                        tn.PopulateOnDemand = true;
                        tn.SelectAction = TreeNodeSelectAction.Select;
                        tn.Expanded = true;
                        trv_Ops.Nodes[0].ChildNodes.Add(tn);
                    }
                    tn.Expand();
                    int i_m = dT.Month;
                    ss = i_m.ToString();
                    sExpandTo += "|" + ss;
                    TreeNode tnprev = tn;
                    tn = trv_Ops.FindNode(sExpandTo);
                    if (tn == null)
                    {
                        tn = new TreeNode(saMonths[i_m - 1], ss);
                        tn.PopulateOnDemand = true;
                        tn.SelectAction = TreeNodeSelectAction.Select;
                        tn.Expanded = true;
                        tnprev.ChildNodes.Add(tn);
                    }
                    tn.Expand();
                    int i_d = dT.Day;
                    ss = i_d.ToString();
                    sExpandTo += "|" + ss;
                    tnprev = tn;
                    tn = trv_Ops.FindNode(sExpandTo);
                    if (tn == null)
                    {
                        tn = new TreeNode("d " + ss, ss);
                        tn.PopulateOnDemand = true;
                        tn.SelectAction = TreeNodeSelectAction.Select;
                        tn.Expanded = true;
                        tnprev.ChildNodes.Add(tn);
                    }
                    TreeNode NewNode = new TreeNode(sNodeText, iID_Op.ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    tn.ChildNodes.Add(NewNode);
                    tn.Expand();
                    sExpandTo += "|" + iID_Op.ToString();
                    tn = trv_Ops.FindNode(sExpandTo);
                    tn.Selected = true;
                    //tnOp = tn;
                    break;
                case enumAddEdit.EditOperation:
                    mCRUD.UpdateOne(Global.enugInfoType.Operations, trv_Ops.SelectedValue, sa);
                    trv_Ops.SelectedNode.Text = "Op " + trv_Ops.SelectedValue + ": " + sNodeText;
                    break;
                default:
                    return;
            }
            ButtonsStyle(trv_Ops.SelectedNode);
            eAddEdit = enumAddEdit.None;
            DaLast[(int)enumLast.LastTakeoff] = dT;
            DaLast[(int)enumLast.LastLanding] = dL;
            Session["DaLast"] = DaLast;
            iaLast[(int)enumLast.LastTakeoffHour] = iTh;
            iaLast[(int)enumLast.LastTakeoffMinute] = iTm;
            iaLast[(int)enumLast.LastLandingHour] = iLh;
            iaLast[(int)enumLast.LastLandingMinute] = iLm;
            Session["iaLast"] = iaLast;
            ValidateOperation(trv_Ops.SelectedNode);
        }

        protected void pbOpDelete_Click(object sender, EventArgs e)
        {
            trv_Ops.SelectedNode.ToolTip = trv_Ops.SelectedNode.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = trv_Ops.SelectedValue; //ID of operation to be deleted
            OkButton.CommandArgument = "Operation";
            lblPopupText.Text = "Are you sure you want to delete this operation? (Also deletes associated flight/equipment/aviator details)";
            MPE_Show(Global.enumButtons.NoYes);
            eAddEdit = enumAddEdit.None;
        }

        protected void pbOpCancel_Click(object sender, EventArgs e)
        {
            eAddEdit = enumAddEdit.None;
        }
        #endregion
        #endregion

        #region Add New / Edit Special Operations *******************

            #region Pushbutton Event Handlers
                protected void pbAddSpOpInfo_Click(object sender, EventArgs e)
                {
                    lblNewSpOpTitle.Text = "Add Special Operations Data to Operation";
                    switch (trv_Ops.SelectedNode.Depth)
                    {
                        case (int)Global.enumDepths.Operation:
                            tnOp = trv_Ops.SelectedNode;
                            break;
                        case (int)Global.enumDepths.OpDetail: // Special Operations has same depth as OpDetail
                            tnOp = trv_Ops.SelectedNode.Parent;
                            break;
                        case (int)Global.enumDepths.Aviator:
                            tnOp = trv_Ops.SelectedNode.Parent.Parent;
                            break;
                    }
                    eAddEdit = enumAddEdit.AddSpecialOp;
                    lblNewSpOpop.Text = tnOp.Text;
                    ModalPopupExt4NewSpOp.Show();
                }

                protected void pbEditSpOpInfo_Click(object sender, EventArgs e)
                {
                    tnOp = trv_Ops.SelectedNode.Parent;
                    if (trv_Ops.SelectedNode.Text != "DELETED")
                    {
                        lblNewODTitle.Text = "Edit Special Operations Data of Operation ";
                        lblNewODop.Text = tnOp.Text;
                        eAddEdit = enumAddEdit.EditSpecialOp;
                        ModalPopupExt4NewSpOp.Show();
                    }
                }

                protected void pbSpOpOK_Click(object sender, EventArgs e)
                {
                    string sNodeText = "Special Operations - ";
                    sa = new string[6];
                    sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
                    sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
                    switch (trv_Ops.SelectedNode.Depth)
                    {
                        case (int)Global.enumDepths.Aviator:
                            sa[2] = trv_Ops.SelectedNode.Parent.Parent.Value;
                            break;
                        case (int)Global.enumDepths.OpDetail:
                            sa[2] = trv_Ops.SelectedNode.Parent.Value;
                            break;
                        case (int)Global.enumDepths.Operation:
                            sa[2] = trv_Ops.SelectedNode.Value;
                            break;
                    }
                    iID_Op = Int32.Parse(sa[2]);
                    DropDownList DDL_SpOpType = (DropDownList)dv_NewSpOp.FindControl("DDL_SpOpType");
                    sa[3] = DDL_SpOpType.SelectedValue.ToString();
                    sNodeText += DDL_SpOpType.SelectedItem.Text;
                    string sDescr = ((TextBox)dv_NewSpOp.FindControl("txbDescr")).Text;
                    if (sDescr.Length < 1)
                    {
                        lblPopupText.Text = "Description must not be empty";
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    sa[4] = sDescr;
                    sNodeText += ": " + sa[4];
                    sa[5] = ((TextBox)dv_NewSpOp.FindControl("txbDuration")).Text;
                    sNodeText += ", Duration " + sa[5] + " minutes";
                    int iDuration = 0;
                    if (!Int32.TryParse(sa[5], out iDuration))
                    {
                        lblPopupText.Text = "Duration '" + sa[5] + "' is not a integer number" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    if (iDuration < 0)
                    {
                        lblPopupText.Text = "Duration '" + sa[5] + "' must not be negative" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
                    var q = (from o in dc.OPERATIONs where o.ID == iID_Op select o).First();
                    int iOperDur = (int)((DateTime)q.DEnd).Subtract((DateTime)q.DBegin).TotalMinutes;
                    if (iDuration > iOperDur)
                    {
                        lblPopupText.Text = "Duration " + sa[5] + " minutes of special operation cannot be longer than entire flight operation duration = " + iOperDur.ToString() + " minutes.";
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    switch (eAddEdit)
                    {
                        case enumAddEdit.AddSpecialOp:
                            int i_ID = 0;
                            mCRUD.InsertOne(Global.enugInfoType.SpecialOp, sa, out i_ID);
                            iID_SpOp = i_ID;
                            // Expand to the just added Special Operations Data
                            DateTime dT = DateTime.Now;
                            string sCmd = "SELECT DBegin FROM OPERATIONS WHERE ID=" + sa[2];
                            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                            {
                                using (SqlCommand cmd = new SqlCommand(sCmd))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    SqlConn.Open();
                                    cmd.Connection = SqlConn;
                                    dT = (DateTime)cmd.ExecuteScalar();
                                }
                            }
                            string sExpandTo = "|" + dT.Year.ToString();
                            TreeNode tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + dT.Month.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + dT.Day.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + sa[2];
                            tn = trv_Ops.FindNode(sExpandTo);
                            TreeNode NewNode = new TreeNode(sNodeText, iID_SpOp.ToString());
                            NewNode.PopulateOnDemand = true;
                            NewNode.SelectAction = TreeNodeSelectAction.Select;
                            NewNode.Expanded = false;
                            tn.ChildNodes.Add(NewNode);
                            tn.Expand();
                            sExpandTo += "|" + iID_SpOp.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Selected = true;
                            ButtonsStyle(tn);
                            break;
                        case enumAddEdit.EditSpecialOp:
                            mCRUD.UpdateOne(Global.enugInfoType.SpecialOp, trv_Ops.SelectedValue, sa);
                            trv_Ops.SelectedNode.Text = sNodeText;
                            break;
                    }
                    eAddEdit = enumAddEdit.None;
                    ValidateOperation(trv_Ops.SelectedNode);
                }

                protected void pbSpOpDelete_Click(object sender, EventArgs e)
                {
                    mCRUD.DeleteOne(Global.enugInfoType.SpecialOp, trv_Ops.SelectedValue);
                    trv_Ops.SelectedNode.ToolTip = trv_Ops.SelectedNode.Text;
                    trv_Ops.SelectedNode.Text = "DELETED";
                    trv_Ops.Nodes.Remove(trv_Ops.SelectedNode);
                    eAddEdit = enumAddEdit.None;
                    ValidateOperation(trv_Ops.SelectedNode);
                }

                protected void pbSpOpCancel_Click(object sender, EventArgs e)
                        {
                            eAddEdit = enumAddEdit.None;
                        }
            #endregion Pushbutton Event Handlers

            #region PreRender Event Handlers
                protected void txbDescr_PreRender(object sender, EventArgs e)
                        {
                            string ss = "";
                            switch (eAddEdit)
                            {
                                case enumAddEdit.AddSpecialOp:
                                    break;
                                case enumAddEdit.EditSpecialOp:
                                    ss = trv_Ops.SelectedNode.Value;
                                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                    {
                                        using (SqlCommand cmd = new SqlCommand(
                                            "SELECT sDescription " +
                                            "FROM SPECIALOPS " +
                                            "WHERE SPECIALOPS.ID = " + ss))
                                        {
                                            cmd.CommandType = CommandType.Text;
                                            cmd.Connection = SqlConn;
                                            SqlConn.Open();
                                            ss = cmd.ExecuteScalar().ToString();
                                        }
                                    }
                                    break;
                                default:
                                    return;
                            }
                            ((TextBox)sender).Text = ss;
                        }

                protected void txbDuration_PreRender(object sender, EventArgs e)
                {
                    string ss = "";
                    switch (eAddEdit)
                    {
                        case enumAddEdit.AddSpecialOp:
                            ss = "10";
                            break;
                        case enumAddEdit.EditSpecialOp:
                            ss = trv_Ops.SelectedNode.Value;
                            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT iDurationMinutes " +
                                    "FROM SPECIALOPS " +
                                    "WHERE SPECIALOPS.ID = " + ss))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = SqlConn;
                                    SqlConn.Open();
                                    ss = cmd.ExecuteScalar().ToString();
                                }
                            }
                            break;
                        default:
                            return;
                    }
                    ((TextBox)sender).Text = ss;
                }

                protected void DDL_SpOpType_PreRender(object sender, EventArgs e)
                {
                    string ss = "";
                    switch (eAddEdit)
                    {
                        case enumAddEdit.AddSpecialOp:
                            ss = mCRUD.GetSetting("DefaultSpecialOperationType");
                            break;
                        case enumAddEdit.EditSpecialOp:
                            ss = trv_Ops.SelectedNode.Value;
                            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT SPECIALOPTYPES.sSpecialOpType " +
                                    "FROM SPECIALOPS INNER JOIN " +
                                        "SPECIALOPTYPES ON SPECIALOPS.iSpecialOpType = SPECIALOPTYPES.ID " +
                                    "WHERE(SPECIALOPS.ID = " + ss + ")"))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = SqlConn;
                                    SqlConn.Open();
                                    ss = (string)cmd.ExecuteScalar();
                                }
                            }
                            break;
                        default:
                            return;
                    }
                    SetDropDownByText((DropDownList)sender, ss);
                }

            #endregion PreRender Event Handlers
        #endregion Add New / Edit Special Operations

        #region Add New / Edit OPDetails *******************
        #region Add New / Edit OpDetail Popup =================
        protected void pbAddEquip_Click(object sender, EventArgs e)
                {
                    lblNewODTitle.Text = "Add Equipment/Aircraft to Operation";
                    switch (trv_Ops.SelectedNode.Depth)
                    {
                        case (int)Global.enumDepths.Operation:
                            tnOp = trv_Ops.SelectedNode;
                            break;
                        case (int)Global.enumDepths.OpDetail:
                            tnOp = trv_Ops.SelectedNode.Parent;
                            break;
                        case (int)Global.enumDepths.Aviator:
                            tnOp = trv_Ops.SelectedNode.Parent.Parent;
                            break;
                    }
                    eAddEdit = enumAddEdit.AddOpDetail;
                    lblNewODop.Text = tnOp.Text;
                    ModalPopupExt4NewOD.Show();
                }

                protected void pbEditEquip_Click(object sender, EventArgs e)
                {
                    tnOp = trv_Ops.SelectedNode.Parent;
                    if (trv_Ops.SelectedNode.Text != "DELETED")
                    {
                        lblNewODTitle.Text = "Edit Equipment/Aircraft of Operation ";
                        lblNewODop.Text = tnOp.Text;
                        eAddEdit = enumAddEdit.EditOpDetail;
                        ModalPopupExt4NewOD.Show();
                    }
                }
                #region PreRender Event Handlers
                    protected void DDL_OpDetail_PreRender(object sender, EventArgs e)
                    {
                        string ss = "";
                        switch (eAddEdit)
                        {
                            case enumAddEdit.AddOpDetail:
                                ss = mCRUD.GetSetting("DefaultEquipment");
                                break;
                            case enumAddEdit.EditOpDetail:
                                ss = trv_Ops.SelectedNode.Value;
                                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                {
                                    using (SqlCommand cmd = new SqlCommand(
                                        "SELECT EQUIPMENT.sShortEquipName " +
                                        "FROM EQUIPMENT INNER JOIN " +
                                            "OPDETAILS ON EQUIPMENT.ID = OPDETAILS.iEquip " +
                                        "WHERE(OPDETAILS.ID = " + ss + ")"))
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = SqlConn;
                                        SqlConn.Open();
                                        ss = (string)cmd.ExecuteScalar();
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                        SetDropDownByText((DropDownList)sender, ss);
                    }

                    protected void DDL_EquipmentRole_PreRender(object sender, EventArgs e)
                    {
                        string ss = "";
                        switch (eAddEdit)
                        {
                            case enumAddEdit.AddOpDetail:
                                ss = mCRUD.GetSetting("DefaultEquipmentRole");
                                break;
                            case enumAddEdit.EditOpDetail:
                                ss = trv_Ops.SelectedNode.Value;
                                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                {
                                    using (SqlCommand cmd = new SqlCommand(
                                        "SELECT EQUIPMENTROLES.sEquipmentRole " +
                                        "FROM OPDETAILS INNER JOIN " +
                                            "EQUIPMENTROLES ON OPDETAILS.iEquipmentRole = EQUIPMENTROLES.ID " +
                                        "WHERE(OPDETAILS.ID = " + ss + ")"))
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = SqlConn;
                                        SqlConn.Open();
                                        ss = (string)cmd.ExecuteScalar();
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                        SetDropDownByText((DropDownList)sender, ss);
                    }

                    protected void txbRelAlt_PreRender(object sender, EventArgs e)
                    {
                        string ss = "";
                        switch (eAddEdit)
                        {
                            case enumAddEdit.AddOpDetail:
                                ss = "4100";
                                break;
                            case enumAddEdit.EditOpDetail:
                                ss = trv_Ops.SelectedNode.Value;
                                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                {
                                    using (SqlCommand cmd = new SqlCommand(
                                        "SELECT dReleaseAltitude " +
                                        "FROM OPDETAILS " +
                                        "WHERE OPDETAILS.ID = " + ss))
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = SqlConn;
                                        SqlConn.Open();
                                        ss = cmd.ExecuteScalar().ToString();
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                        ((TextBox)sender).Text = ss;
                    }

                    protected void txbMaxAlt_PreRender(object sender, EventArgs e)
                    {
                        string ss = "";
                        switch (eAddEdit)
                        {
                            case enumAddEdit.AddOpDetail:
                                ss = "4100";
                                break;
                            case enumAddEdit.EditOpDetail:
                                ss = trv_Ops.SelectedNode.Value;
                                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                {
                                    using (SqlCommand cmd = new SqlCommand(
                                        "SELECT dMaxAltitude " +
                                        "FROM OPDETAILS " +
                                        "WHERE OPDETAILS.ID = " + ss))
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = SqlConn;
                                        SqlConn.Open();
                                        ss = cmd.ExecuteScalar().ToString();
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                        ((TextBox)sender).Text = ss;
                    }

                #endregion PreRender Event Handlers

                protected void pbODOK_Click(object sender, EventArgs e)
                {
                    string sNodeText = "";
                    // sa = new string[7]; SCR 217
                    sa = new string[8]; // SCR 217
                    sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
                    sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
                    sa[2] = ((DropDownList)dv_NewOD.FindControl("DDL_OpDetail")).SelectedValue.ToString();
                    switch (trv_Ops.SelectedNode.Depth)
                    {
                        case (int)Global.enumDepths.Aviator:
                            sa[3] = trv_Ops.SelectedNode.Parent.Parent.Value;
                            break;
                        case (int)Global.enumDepths.OpDetail:
                            sa[3] = trv_Ops.SelectedNode.Parent.Value;
                            break;
                        case (int)Global.enumDepths.Operation:
                            sa[3] = trv_Ops.SelectedNode.Value;
                            break;
                    }
                    iID_Op = Int32.Parse(sa[3]);
                    sa[4] = ((DropDownList)dv_NewOD.FindControl("DDL_EquipmentRole")).SelectedValue.ToString();
                    sNodeText = ((DropDownList)dv_NewOD.FindControl("DDL_EquipmentRole")).SelectedItem.Text;
                    sNodeText += ": " + ((DropDownList)dv_NewOD.FindControl("DDL_OpDetail")).SelectedItem.Text;
                    sa[5] = ((TextBox)dv_NewOD.FindControl("txbMaxAlt")).Text;
                    sa[6] = ((TextBox)dv_NewOD.FindControl("txbRelAlt")).Text;
                    sa[7] = "600"; // SCR 217
                    sNodeText += ", Rel Alt " + sa[6];
                    sNodeText += ", Max Alt " + sa[5];
                    Decimal dMaxAlt = 0.0M;
                    if (!Decimal.TryParse(sa[5], out dMaxAlt))
                    {
                        lblPopupText.Text = "Max Altitude '" + sa[5] + "' is not a decimal number" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    if (dMaxAlt < -1000.00M || dMaxAlt > 100000.00M)
                    {
                        lblPopupText.Text = "Max Altitude '" + sa[5] + "' is not between -1000 and 100000" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    sa[5] = Decimal.Round(dMaxAlt, 0).ToString();
                    Decimal dRelAlt = 0.0M;
                    if (!Decimal.TryParse(sa[6], out dRelAlt))
                    {
                        lblPopupText.Text = "Release Altitude '" + sa[6] + "' is not a decimal number" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    if (dRelAlt < -1000.00M || dRelAlt > 100000.00M)
                    {
                        lblPopupText.Text = "Release Altitude '" + sa[6] + "' is not between -1000 and 100000" + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    sa[6] = Decimal.Round(dRelAlt, 0).ToString();
                    if (dRelAlt > dMaxAlt)
                    {
                        lblPopupText.Text = "Release Altitude " + sa[6] + " cannot be greater than Max Altitude " + sa[5] + scEdUpCanceled;
                        MPE_Show(Global.enumButtons.OkOnly);
                        return;
                    }
                    switch (eAddEdit)
                    {
                        case enumAddEdit.AddOpDetail:
                            int i_ID = 0;
                            mCRUD.InsertOne(Global.enugInfoType.OpDetail, sa, out i_ID);
                            iID_OD = i_ID;
                            // Expand to the just added OpDetail
                            DateTime dT = DateTime.Now;
                            string sCmd = "SELECT DBegin FROM OPERATIONS WHERE ID=" + sa[3];
                            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                            {
                                using (SqlCommand cmd = new SqlCommand(sCmd))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    SqlConn.Open();
                                    cmd.Connection = SqlConn;
                                    dT = (DateTime)cmd.ExecuteScalar();
                                }
                            }
                            string sExpandTo = "|" + dT.Year.ToString();
                            TreeNode tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + dT.Month.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + dT.Day.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Expand();
                            sExpandTo += "|" + sa[3];
                            tn = trv_Ops.FindNode(sExpandTo);
                            TreeNode NewNode = new TreeNode(sNodeText, iID_OD.ToString());
                            NewNode.PopulateOnDemand = true;
                            NewNode.SelectAction = TreeNodeSelectAction.Select;
                            NewNode.Expanded = false;
                            tn.ChildNodes.Add(NewNode);
                            tn.Expand();
                            sExpandTo += "|" + iID_OD.ToString();
                            tn = trv_Ops.FindNode(sExpandTo);
                            tn.Selected = true;
                            ButtonsStyle(tn);
                            break;
                        case enumAddEdit.EditOpDetail:
                            mCRUD.UpdateOne(Global.enugInfoType.OpDetail, trv_Ops.SelectedValue, sa);
                            trv_Ops.SelectedNode.Text = sNodeText;
                            break;
                    }
                    eAddEdit = enumAddEdit.None;
                    ValidateOperation(trv_Ops.SelectedNode);
                }

                protected void pbODDelete_Click(object sender, EventArgs e)
                {
                    mCRUD.DeleteOne(Global.enugInfoType.OpDetail, trv_Ops.SelectedValue);
                    trv_Ops.SelectedNode.ToolTip = trv_Ops.SelectedNode.Text;
                    trv_Ops.SelectedNode.Text = "DELETED";
                    trv_Ops.Nodes.Remove(trv_Ops.SelectedNode);
                    eAddEdit = enumAddEdit.None;
                    ValidateOperation(trv_Ops.SelectedNode);
                }

                protected void pbODCancel_Click(object sender, EventArgs e)
                {
                    eAddEdit = enumAddEdit.None;
                }
            #endregion New/Edit OpDetail Popup
        #endregion ADD New / Edit OpDetail

        #region New/Edit Aviator ********************
        protected void pbAddAviator_Click(object sender, EventArgs e)
        {
            lblAvTitle.Text = "Add Aviator/Operator to Aircraft/Equipment ";
            switch (trv_Ops.SelectedNode.Depth)
            {
                case (int)Global.enumDepths.Aviator:
                    tnOD = trv_Ops.SelectedNode.Parent;
                    break;
                case (int)Global.enumDepths.OpDetail:
                    tnOD = trv_Ops.SelectedNode;
                    break;
            }
            lblAvSubtitle.Text = tnOD.Text;
            eAddEdit = enumAddEdit.AddAviator;
            ModalPopupExt4NewAv.Show();
        }

        protected void pbEditAviator_Click(object sender, EventArgs e)
        {
            tnOD = trv_Ops.SelectedNode.Parent;
            if (trv_Ops.SelectedNode.Text != "DELETED")
            {
                lblAvTitle.Text = "Edit Aviator/Operator of Aircraft/Equipment ";
                lblAvSubtitle.Text = tnOD.Text;
                eAddEdit = enumAddEdit.EditAviator;
                ModalPopupExt4NewAv.Show();
            }
        }

        #region New/Edit Aviator Popup ==================
        #region PreRender Event Handlers
        protected void DDL_Av_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    ss = mCRUD.GetSetting("DefaultAviator");
                    break;
                case enumAddEdit.EditAviator:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT PEOPLE.sDisplayName " +
                            "FROM AVIATORS INNER JOIN PEOPLE ON AVIATORS.iPerson = PEOPLE.ID " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }

        protected void DDL_AvRole_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    ss = mCRUD.GetSetting("DefaultAviatorRole");
                    break;
                case enumAddEdit.EditAviator:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT AVIATORROLES.sAviatorRole " +
                            "FROM AVIATORROLES INNER JOIN AVIATORS ON AVIATORROLES.ID = AVIATORS.iAviatorRole " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = (string)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            SetDropDownByText((DropDownList)sender, ss);
        }

        protected void txbPercChrg_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    ss = "0";
                    break;
                case enumAddEdit.EditAviator:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT dPercentCharge " +
                            "FROM AVIATORS " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = cmd.ExecuteScalar().ToString();
                        }
                    }
                    break;
                default:
                    return;
            }
            ((TextBox)sender).Text = ss;
        }

        protected void chb1stFlt_PreRender(object sender, EventArgs e)
        {
            bool b1st = false;
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    break;
                case enumAddEdit.EditAviator:
                    string ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT b1stFlight " +
                            "FROM AVIATORS " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            b1st = (bool)cmd.ExecuteScalar();
                        }
                    }
                    break;
                default:
                    return;
            }
            ((CheckBox)sender).Checked = b1st;
        }

        protected void txbmInvoiced_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    ss = "0";
                    break;
                case enumAddEdit.EditAviator:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT mInvoiced " +
                            "FROM AVIATORS " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            ss = cmd.ExecuteScalar().ToString();
                        }
                    }
                    break;
                default:
                    return;
            }
            ((TextBox)sender).Text = ss;
        }

        protected void txbDInvoiced_PreRender(object sender, EventArgs e)
        {
            string ss = "";
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    ss = "";
                    break;
                case enumAddEdit.EditAviator:
                    ss = trv_Ops.SelectedNode.Value;
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT DInvoiced " +
                            "FROM AVIATORS " +
                            "WHERE AVIATORS.ID = " + ss))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            SqlConn.Open();
                            object o = cmd.ExecuteScalar();
                            if (o == DBNull.Value)
                            {
                                ss = "";
                            }
                            else
                            {
                                ss = CustFmt.sFmtDate((DateTimeOffset)o, CustFmt.enDFmt.DateOnly).Replace("/", "-");
                            }
                        }
                    }
                    break;
                default:
                    return;
            }
            ((TextBox)sender).Text = ss;
        }
        #endregion PreRender Event Handlers

        protected void pbAvOK_Click(object sender, EventArgs e)
        {
            string sNodeText = "";
            sa = new string[9];
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[2] = ((DropDownList)dv_NewAv.FindControl("DDL_Av")).SelectedValue.ToString();
            sa[3] = iID_OD.ToString();
            sa[4] = ((DropDownList)dv_NewAv.FindControl("DDL_AvRole")).SelectedValue.ToString();
            sNodeText = ((DropDownList)dv_NewAv.FindControl("DDL_AvRole")).SelectedItem.Text;
            sNodeText += ": " + ((DropDownList)dv_NewAv.FindControl("DDL_Av")).SelectedItem.Text;
            sa[5] = ((TextBox)dv_NewAv.FindControl("txbPercChrg")).Text;
            Decimal dr = 0.0M;
            if (!Decimal.TryParse(sa[5], out dr))
            {
                lblPopupText.Text = "'" + sa[5] + "' is not a decimal number";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            if (dr < 0.00M || dr > 100.00M)
            {
                lblPopupText.Text = "'" + sa[5] + "' is not between 0.00 and 100.00";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            sa[5] = Decimal.Round(dr, 2).ToString();
            sNodeText += ", % Charge = " + sa[5];
            bool b1st = ((CheckBox)dv_NewAv.FindControl("chb1stFlt")).Checked;
            sa[6] = b1st ? "1" : "0";
            sNodeText += b1st ? ", First Flight of season with Instructor" : "";
            sa[7] = ((TextBox)dv_NewAv.FindControl("txbmInvoiced")).Text;
            sNodeText += ", $invoiced = " + decimal.Parse(sa[7]).ToString("F2");
            sa[8] = ((TextBox)dv_NewAv.FindControl("txbDInvoiced")).Text.Replace("-","/");
            sNodeText += ", Date Invoiced = " + sa[8];
            switch (eAddEdit)
            {
                case enumAddEdit.AddAviator:
                    int i_ID = 0;
                    mCRUD.InsertOne(Global.enugInfoType.Aviators, sa, out i_ID);
                    iID_Av = i_ID;
                    //Expand to the just added Aviator
                    DateTime dT = DateTime.Now;
                    string sCmd =
                        "SELECT OPERATIONS.ID, OPERATIONS.DBegin " +
                        "FROM OPDETAILS INNER JOIN OPERATIONS ON OPDETAILS.iOperation = OPERATIONS.ID " +
                        "WHERE OPDETAILS.ID = " + sa[3];
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (DataTable dtbl = new DataTable())
                        {
                            using (SqlCommand cmd = new SqlCommand(sCmd))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = CommandType.Text;
                                    SqlConn.Open();
                                    cmd.Connection = SqlConn;
                                    sda.SelectCommand = cmd;
                                    sda.Fill(dtbl);
                                }
                            }
                            dT = (DateTime)dtbl.Rows[0]["DBegin"];
                            iID_Op = (int)dtbl.Rows[0]["ID"];
                        }
                    }
                    string sExpandTo = "|" + dT.Year.ToString();
                    TreeNode tn = trv_Ops.FindNode(sExpandTo);
                    tn.Expand();
                    sExpandTo += "|" + dT.Month.ToString();
                    tn = trv_Ops.FindNode(sExpandTo);
                    tn.Expand();
                    sExpandTo += "|" + dT.Day.ToString();
                    tn = trv_Ops.FindNode(sExpandTo);
                    tn.Expand();
                    sExpandTo += "|" + iID_Op.ToString();
                    tn = trv_Ops.FindNode(sExpandTo);
                    tn.Expand();
                    sExpandTo += "|" + sa[3];
                    tn = trv_Ops.FindNode(sExpandTo);
                    TreeNode NewNode = new TreeNode(sNodeText, iID_Av.ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    tn.ChildNodes.Add(NewNode);
                    tn.Expand();
                    sExpandTo += "|" + iID_Av.ToString();
                    tn = trv_Ops.FindNode(sExpandTo);
                    tn.Selected = true;
                    ButtonsStyle(tn);
                    break;
                case enumAddEdit.EditAviator:
                    mCRUD.UpdateOne(Global.enugInfoType.Aviators, trv_Ops.SelectedValue, sa);
                    trv_Ops.SelectedNode.Text = sNodeText;
                    break;
                default:
                    return;
            }
            eAddEdit = enumAddEdit.None;
            ValidateOperation(trv_Ops.SelectedNode);
        }

        protected void pbAvCancel_Click(object sender, EventArgs e)
        {
            eAddEdit = enumAddEdit.None;
        }

        protected void pbAvDelete_Click(object sender, EventArgs e)
        {
            mCRUD.DeleteOne(Global.enugInfoType.Aviators, trv_Ops.SelectedNode.Value);
            // It's hard to make the node disappear - you have to refresh the entire TreeView.
            // Instead, we just show its deleted status. Will disappear when user refreshes tree
            // for other reasons (like navigating away from this page and coming back to it).
            trv_Ops.SelectedNode.ToolTip = trv_Ops.SelectedNode.Text;
            trv_Ops.SelectedNode.Text = "DELETED";
            trv_Ops.Nodes.Remove(trv_Ops.SelectedNode);
            eAddEdit = enumAddEdit.None;
            ValidateOperation(trv_Ops.SelectedNode);
        }
        #endregion New Aviator Popup
        #endregion New/Edit Aviator

        private void ButtonsStyle(TreeNode utn)
        {
            switch (utn.Depth)
            {
                case 4: // Operation
                    pbAddOp.CssClass = "buttonEnabled";
                    pbAddOp.Enabled = true;
                    pbAddSpOpInfo.CssClass = "buttonEnabled";
                    pbAddSpOpInfo.Enabled = true;
                    pbAddEquip.CssClass = "buttonEnabled";
                    pbAddEquip.Enabled = true;
                    pbAddAviator.CssClass = "buttonDisabled";
                    pbAddAviator.Enabled = false;
                    pbEditOp.CssClass = "buttonEnabled";
                    pbEditOp.Enabled = true;
                    pbEditSpOpInfo.CssClass = "buttonDisabled";
                    pbEditSpOpInfo.Enabled = false;
                    pbEditEquip.CssClass = "buttonDisabled";
                    pbEditEquip.Enabled = false;
                    pbEditAviator.CssClass = "buttonDisabled";
                    pbEditAviator.Enabled = false;
                    break;
                case 5: // Special Operations Info, or Operation Details
                    pbAddOp.CssClass = "buttonEnabled";
                    pbAddOp.Enabled = true;
                    pbAddSpOpInfo.CssClass = "buttonEnabled";
                    pbAddSpOpInfo.Enabled = true;
                    pbEditOp.CssClass = "buttonDisabled";
                    pbEditOp.Enabled = false;

                    int iOper = Int32.Parse(utn.Parent.Value);
                    StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
                    // Does this node represent a special operations info record?
                    if (utn.Text.StartsWith("Special Operations -"))
                    {
                        pbEditSpOpInfo.CssClass = "buttonEnabled";
                        pbEditSpOpInfo.Enabled = true;
                        pbEditEquip.CssClass = "buttonDisabled";
                        pbEditEquip.Enabled = false;
                        pbAddAviator.CssClass = "buttonDisabled";
                        pbAddAviator.Enabled = false;
                    }
                    else
                    {
                        pbEditSpOpInfo.CssClass = "buttonDisabled";
                        pbEditSpOpInfo.Enabled = false;
                        pbEditEquip.CssClass = "buttonEnabled";
                        pbEditEquip.Enabled = true;
                        pbAddEquip.CssClass = "buttonEnabled";
                        pbAddEquip.Enabled = true;
                        pbAddAviator.CssClass = "buttonEnabled";
                        pbAddAviator.Enabled = true;
                    }

                    pbEditAviator.CssClass = "buttonDisabled";
                    pbEditAviator.Enabled = false;
                    break;
                case 6: // Aviators
                    pbAddOp.CssClass = "buttonEnabled";
                    pbAddOp.Enabled = true;
                    pbAddSpOpInfo.CssClass = "buttonEnabled";
                    pbAddSpOpInfo.Enabled = true;
                    pbAddEquip.CssClass = "buttonEnabled";
                    pbAddEquip.Enabled = true;
                    pbAddAviator.CssClass = "buttonEnabled";
                    pbAddAviator.Enabled = true;
                    pbEditOp.CssClass = "buttonDisabled";
                    pbEditOp.Enabled = false;
                    pbEditSpOpInfo.CssClass = "buttonDisabled";
                    pbEditSpOpInfo.Enabled = false;
                    pbEditEquip.CssClass = "buttonDisabled";
                    pbEditEquip.Enabled = false;
                    pbEditAviator.CssClass = "buttonEnabled";
                    pbEditAviator.Enabled = true;
                    break;
                default: // everything else, including Root, Year, Month, Day
                    pbAddOp.CssClass = "buttonEnabled";
                    pbAddOp.Enabled = true;
                    pbAddSpOpInfo.CssClass = "buttonDisabled";
                    pbAddSpOpInfo.Enabled = false;
                    pbAddEquip.CssClass = "buttonDisabled";
                    pbAddEquip.Enabled = false;
                    pbAddAviator.CssClass = "buttonDisabled";
                    pbAddAviator.Enabled = false;
                    pbEditOp.CssClass = "buttonDisabled";
                    pbEditOp.Enabled = false;
                    pbEditSpOpInfo.CssClass = "buttonDisabled";
                    pbEditSpOpInfo.Enabled = false;
                    pbEditEquip.CssClass = "buttonDisabled";
                    pbEditEquip.Enabled = false;
                    pbEditAviator.CssClass = "buttonDisabled";
                    pbEditAviator.Enabled = false;
                    break;
            }
        }
        public void SetDropDownByText(DropDownList ddl, string sText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == sText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddl.SelectedItem.Text = sText;
        }

        private void ValidateOperation(TreeNode tn)
        {
            TreeNode tn_Op = tn;
            switch (tn.Depth)
            {
                case (int)Global.enumDepths.Operation:
                    break;
                case (int)Global.enumDepths.OpDetail:
                    tn_Op = tn.Parent;
                    break;
                case (int)Global.enumDepths.Aviator:
                    tn_Op = tn.Parent.Parent;
                    break;
                default:
                    tn_Op = null;
                    break;
            }
            if (tn_Op != null && tn_Op.Text != "DELETED")
            {
                decimal dSum = 0.00M;
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT SUM(AVIATORS.dPercentCharge) AS SumPercChrg " +
                        "FROM AVIATORS INNER JOIN " +
                            "OPDETAILS ON AVIATORS.iOpDetail = OPDETAILS.ID INNER JOIN " +
                            "OPERATIONS ON OPDETAILS.iOperation = OPERATIONS.ID " +
                        "WHERE OPERATIONS.ID = " + tn_Op.Value))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        SqlConn.Open();
                        object o = cmd.ExecuteScalar();
                        if (o.GetType() == dSum.GetType())
                        {
                            dSum = (Decimal)o;
                        }
                    }
                }
                string st = tn_Op.Text;
                if (dSum == 100.00M)
                {
                    tn_Op.ToolTip = "";
                    if (st.Substring(0, 5) == "<span")
                    {//remove <span> markup
                        int iEnd = st.IndexOf(">");
                        st = st.Substring(iEnd+1);
                        iEnd = st.IndexOf("</span>");
                        st = st.Substring(0, iEnd);
                        tn_Op.Text = st;
                    }
                }
                else
                {
                    if (st.Substring(0, 5) != "<span")
                    {
                        st = "<span style='color:red; font-weight:bold'>" + st + "</span>";
                        tn_Op.Text = st;
                        tn_Op.ToolTip = "Sum of percent charges to each aviator is not = 100.00 but " + dSum.ToString();
                    }
                }
            }
        }
    }
}