using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer
{
    public partial class DBProperties : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (DataTable dt0 = new DataTable())
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT CONVERT(nvarchar(50), SERVERPROPERTY('PRODUCTLEVEL')) AS [Product Level], " +
                            "CONVERT(nvarchar(50),SERVERPROPERTY('PRODUCTVERSION')) AS [Product Version];"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = SqlConn;
                                sda.SelectCommand = cmd;
                                sda.Fill(dt0);
                            }
                        }
                        gvSQLProps.DataSource = dt0;
                        gvSQLProps.DataBind();
                    }
                    using (DataTable dt = new DataTable())
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT name, physical_name, state_desc, size/128.0 FileSizeInMB, " +
                            "size / 128.0 - CAST(FILEPROPERTY(name, 'SpaceUsed') AS int) / 128.0 " +
                            "AS EmptySpaceInMB FROM sys.database_files;"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = SqlConn;
                                sda.SelectCommand = cmd;
                                sda.Fill(dt);
                            }
                        }
                        gvDBProps.DataSource = dt;
                        gvDBProps.DataBind();
                    }

                }
            }
        }
    }
}