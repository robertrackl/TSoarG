using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.AdminPages.Security
{
    public partial class ManageRoles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                DisplayRolesInGrid();
        }

        protected void CreateRoleButton_Click(object sender, EventArgs e)
        {
            string newRoleName = Server.HtmlEncode(RoleName.Text.Trim());
            if (!System.Web.Security.Roles.RoleExists(newRoleName))
            {// Create the role
                System.Web.Security.Roles.CreateRole(newRoleName);
            }
            RoleName.Text = string.Empty;
            DisplayRolesInGrid();
        }
        private void DisplayRolesInGrid()
        {
            RoleList.DataSource = System.Web.Security.Roles.GetAllRoles();
            RoleList.DataBind();
        }
        protected void RoleList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Get the RoleNameLabel
            Label RoleNameLabel = RoleList.Rows[e.RowIndex].FindControl("RoleNameLabel") as Label;

            // Delete the role
            System.Web.Security.Roles.DeleteRole(RoleNameLabel.Text, false);

            // Rebind the data to the RoleList grid
            DisplayRolesInGrid();
        }
        protected void RoleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // reference the Delete LinkButton
                LinkButton db = (LinkButton)e.Row.Cells[0].Controls[0];

                // Get information about the product bound to the row
                string srole = e.Row.DataItem.ToString();

                db.OnClientClick = string.Format(
                    "return confirm('Are you certain you want to delete the {0} role?');",
                    srole);
            }
        }
    }
}