using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.AdminPages.Security
{
    public partial class EditUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindUserGrid();
        }
        private void BindUserGrid()
        {
            System.Web.Security.MembershipUserCollection allUsers = System.Web.Security.Membership.GetAllUsers();
            UserGrid.DataSource = allUsers;
            UserGrid.DataBind();
        }
        protected void UserGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Set the grid's EditIndex and rebind the data

            UserGrid.EditIndex = e.NewEditIndex;
            BindUserGrid();
        }

        protected void UserGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Revert the grid's EditIndex to -1 and rebind the data
            UserGrid.EditIndex = -1;
            BindUserGrid();
        }

        protected void UserGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Exit if the page is not valid
            if (!Page.IsValid)
                return;

            // Determine the username of the user we are editing
            string UserName = UserGrid.DataKeys[e.RowIndex].Value.ToString();

            // Read in the entered information and update the user
            TextBox EmailTextBox = (TextBox)UserGrid.Rows[e.RowIndex].FindControl("Email");
            TextBox CommentTextBox = (TextBox)UserGrid.Rows[e.RowIndex].FindControl("Comment");

            // Return information about the user
            System.Web.Security.MembershipUser UserInfo = System.Web.Security.Membership.GetUser(UserName);

            // Update the User account information
            UserInfo.Email = Server.HtmlEncode(EmailTextBox.Text.Trim());
            UserInfo.Comment = Server.HtmlEncode(CommentTextBox.Text.Trim());

            System.Web.Security.Membership.UpdateUser(UserInfo);

            // Revert the grid's EditIndex to -1 and rebind the data
            UserGrid.EditIndex = -1;
            BindUserGrid();
        }
        protected void UserGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Determine the username of the user we are editing
            string UserName = UserGrid.DataKeys[e.RowIndex].Value.ToString();

            // Delete the user
            System.Web.Security.Membership.DeleteUser(UserName);

            // Revert the grid's EditIndex to -1 and rebind the data
            UserGrid.EditIndex = -1;
            BindUserGrid();
        }
    }
}