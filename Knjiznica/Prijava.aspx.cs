using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Knjiznica
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Cant log in while logged in
            if (Session["User"] != null && !string.IsNullOrEmpty(Session["User"].ToString()))
            {
                Response.Redirect("Home.aspx");
            }
        }

        protected void Prijava_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            lblResult.Visible = false;
            lblResult.ForeColor = System.Drawing.Color.Red;

            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // First, check if user exists
                    string checkUserQuery = "SELECT Geslo FROM Uporabnik WHERE Ime = @username";

                    using (SqlCommand cmd = new SqlCommand(checkUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            lblResult.Visible = true;
                            lblResult.Text = "Uporabnik ne obstaja.";
                        }
                        else
                        {
                            string databasePassword = result.ToString();
                            string hashpassword = HashCode(password);

                            if (databasePassword == hashpassword)
                            {
                                //User session start
                                Session["User"] = txtUsername.Text;
                                Response.Redirect("MyBooks.aspx");
                            }
                            else
                            {
                                lblResult.Visible = true;
                                lblResult.Text = "Napačno geslo.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri prijavi: " + ex.Message;
            }
        }
        private static string HashCode(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}