using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Equipment.EquipAging
{
    public partial class EqAgingParSets : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Must create a default parameter set if no parameter sets exist. Otherwise, the user has no way of creating one.
            EquipmentDataContext eqdc = new EquipmentDataContext();
            var qt = from t in eqdc.EQUIPACTIONTYPEs where t.sEquipActionType == "Documentation" select t.ID;
            int iEquipActType = 0;
            foreach (int i in qt)
            {
                iEquipActType = i;
            }
            if (iEquipActType < 1)
            {
                ProcessPopupException(new Global.excToPopup("The Equipment Action Type `Documentation` does not exist; please create it."));
                return;
            }
            if ((from p in eqdc.EQUIPAGINGPARs select p).Count() < 1)
            {
                EQUIPAGINGPAR p = new EQUIPAGINGPAR()
                {
                    sShortDescript = "Default Parameter Set",
                    iEquipActionType = iEquipActType,
                    iIntervalElapsed = -1,
                    sTimeUnitsElapsed = "Months",
                    cDeadLineMode = 'C',
                    iDeadLineSpt1 = -1,
                    iDeadLineSpt2 = -1,
                    iIntervalOperating = -1,
                    sTimeUnitsOperating = "Hours",
                    iIntervalDistance = -1,
                    sDistanceUnits = "Miles",
                    iIntervalCycles = -1,
                    sComment = "Default parameter set; feel free to delete if not used, and if other parameter sets exist."
                };
                eqdc.EQUIPAGINGPARs.InsertOnSubmit(p);
                eqdc.SubmitChanges();
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
            if (btn.ID == "YesButton")
            {
                if (btn.CommandName == "Delete")
                {

                    switch (OkButton.CommandArgument)
                    {
                        case "ParSet":
                            // Delete the parameter set
                            try
                            {
                                mCRUD.DeleteOne(Global.enugInfoType.EquipParameters, btn.CommandArgument);
                            }
                            catch (Global.excToPopup exc)
                            {
                                ProcessPopupException(exc);
                            }
                            gvEqParsMaster.DataBind();
                            break;
                    }
                }
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void dvEqPars_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            // Refresh the GridView control after a new record is inserted 
            // in the DetailsView control.
            gvEqParsMaster.DataBind();
        }

        protected void dvEqPars_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            for (int i = 0; i < e.Values.Count; i++)
            {
                object v = e.Values[i];
                Type t = v.GetType();
                if (t.Equals(typeof(string)))
                {
                    e.Values[i] = Server.HtmlEncode((string)v);
                }
            }
        }

        protected void dvEqPars_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            gvEqParsMaster.DataBind();
        }

        protected void dvEqPars_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            for (int i = 0; i < e.NewValues.Count; i++)
            {
                object v = e.NewValues[i];
                if (v != null)
                {
                    Type t = v.GetType();
                    if (t.Equals(typeof(string)))
                    {
                        e.NewValues[i] = Server.HtmlEncode((string)v);
                    }
                }
            }
        }

        protected void gvEqParsMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEqParsMaster.DataBind();
        }

        protected void gvEqParsMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == gvEqParsMaster.SelectedIndex)
                {
                    e.Row.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }

        protected void dvEqPars_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sMsg = "You should probably NOT DELETE this equipment aging parameter set record because it is referenced from other data tables. " +
                "Those other records would also be wiped out (and quite possibly more data if there are more related tables): ";
            int iCount = 0;
            string sID = ((Label)((DetailsView)sender).Rows[e.RowIndex].FindControl("lblIID")).Text;
            int iID = Int32.Parse(sID);
            EquipmentDataContext eqdc = new EquipmentDataContext();
            iCount = (from p in eqdc.EQUIPAGINGPARs select p).Count();
            if (iCount < 2)
            {
                ProcessPopupException(new Global.excToPopup("Cannot delete the last parameter set. At least one must exist."));
                return;
            }

            iCount = (from c in eqdc.EQUIPAGINGITEMs where c.iParam == iID select c).Count();
            if (iCount > 0)
            {
                ProcessPopupException(new Global.excToPopup("Parameter Set with ID=" + sID + " is referenced by one or more aging items. Cannot delete."));
                return;
            }

            try
            {
                // Note written on 2021/1/10: Control will never arrive here unless, in the future, table EQUIPAGINGPARS will be referenced
                //   from table(s) other than EQUIPAGINGITEMS.
                var dtEq = eqdc.spForeignKeyRefs("EQUIPAGINGPARS", iID);
                foreach (var row in dtEq)
                {
                    iCount += (int)row.iNumFKRefs;
                    sMsg += row.iNumFKRefs.ToString() + " times in " + row.sFKTable + ", ";
                }
                sMsg = sMsg.Substring(0, sMsg.Length - 2) + ". You should click on `No` unless you really know what you are doing!";
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
                return;
            }

            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sID;
            OkButton.CommandArgument = "ParSet";
            if (iCount < 1)
            {
                lblPopupText.Text = "Please confirm deletion of equipment component record with internal Id " + YesButton.CommandArgument;
            }
            else
            {
                lblPopupText.Text = sMsg;
            }
            MPE_Show(Global.enumButtons.NoYes);
        }
    }
}
