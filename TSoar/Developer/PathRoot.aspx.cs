using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer
{
    public partial class PathRoot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblPath.Text = System.Environment.CurrentDirectory;
            lblRoot.Text= System.IO.Path.GetPathRoot(Environment.CurrentDirectory);
            lblAppPath1.Text = System.Web.HttpRuntime.AppDomainAppPath;
            lblAppPath2.Text = System.Web.HttpContext.Current.Server.MapPath("~");
        }
    }
}