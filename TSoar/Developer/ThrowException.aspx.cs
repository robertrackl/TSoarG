using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer
{
    public partial class ThrowException : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void pbGeneral_Click(object sender, EventArgs e)
        {
            throw new Exception("From ThrowException.aspx.cs: `General` exception thrown on purpose");
        }

        protected void pbexcToPopup_Click(object sender, EventArgs e)
        {
            Global.excToPopup etp = new Global.excToPopup("From ThrowException.aspx.cs: `excToPopup` exception thrown on purpose");
            throw etp;
        }

        protected void pbexcToPopupInner_Click(object sender, EventArgs e)
        {
            Exception Inner = new Exception("Artificial Inner Exception to go with purposely thrown excToPopup");
            Global.excToPopup etp = new Global.excToPopup("From ThrowException.aspx.cs: `excToPopup` exception thrown on purpose, with Inner Exception", Inner);
            throw etp;
        }

        protected void pbHttpExc_Click(object sender, EventArgs e)
        {
            throw new HttpException(477, "From ThrowException.aspx.cs: 'HttpException' thrown on purpose with unassigned error code 477");
        }
    }
}