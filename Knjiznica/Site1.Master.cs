using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace Knjiznica
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected string GetActiveClass(string pageName)
        {
            string currentPage = System.IO.Path.GetFileName(Request.Path);
            return string.Equals(currentPage, pageName, StringComparison.OrdinalIgnoreCase) ? "active" : "";
        }

        private static string activeConnectionString = null;

        public string GetActiveConnectionString()
        {
            if (!string.IsNullOrEmpty(activeConnectionString))
            {
                return activeConnectionString;
            }

            var MdfConnection = System.Configuration.ConfigurationManager.ConnectionStrings["MdfConnection"].ConnectionString;
            var MSSQLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;

            if (TestConnection(MdfConnection))
            {
                activeConnectionString = MdfConnection;
            }
            else if (TestConnection(MSSQLConnection))
            {
                activeConnectionString = MSSQLConnection;
            }
            else
            {
                throw new Exception("Ni povezave z database.");
            }

            return activeConnectionString;
        }

        private bool TestConnection(string connStr)
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}