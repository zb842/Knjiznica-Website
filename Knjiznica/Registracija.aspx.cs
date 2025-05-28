using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Knjiznica
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Cant register while logged in
            if (Session["User"] != null && !string.IsNullOrEmpty(Session["User"].ToString()))
            {
                Response.Redirect("Home.aspx");
            }
        }

        protected void Registracija_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            lblResult.Visible = false;
            lblResult.ForeColor = System.Drawing.Color.Red;

            if (!IsValidUsername(username, out string errorUsername))
            {
                lblResult.Visible = true;
                lblResult.Text = errorUsername;
                return;
            }
            else if (UsernameExists(username))
            {
                lblResult.Visible = true;
                lblResult.Text = "Uporabniško ime že obstaja.";
                return;
            }

            if (!IsValidPassword(password, out string errorPassword))
            {
                lblResult.Visible = true;
                lblResult.Text = errorPassword;
                return;
            }

            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                string hashPassword = HashCode(password);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    //Database add entry(Uporabnik)
                    string query = "INSERT INTO Uporabnik ([Ime], [Geslo]) VALUES (@username, @password)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashPassword);

                        cmd.ExecuteNonQuery();
                    }

                }

                //User session start
                Session["User"] = txtUsername.Text;
                Response.Redirect("MyBooks.aspx");
            }
            catch (Exception ex)
            {
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri registraciji: " + ex.Message;
            }
        }


        private bool IsValidPassword(string password, out string error)
        {
            error = "";
            if (string.IsNullOrEmpty(password))
            {
                error = "Geslo ne sme biti prazno.";
                return false;
            }

            if (password.Length < 3 || password.Length > 20)
            {
                error = "Geslo mora biti med 3 in 20 znaki.";
                return false;
            }

            foreach (char x in password)
            {
                if (!char.IsLetterOrDigit(x))
                {
                    error = "Geslo sme vsebovati samo črke in številke.";
                    return false;
                }
            }

            return true;
        }


        private bool IsValidUsername(string username, out string error)
        {
            error = "";
            if (string.IsNullOrEmpty(username))
            {
                error = "Uporabniško ime ne sme biti prazno.";
                return false;
            }

            if (username.Length < 3 || username.Length > 20)
            {
                error = "Uporabniško ime mora biti med 3 in 20 znaki.";
                return false;
            }

            foreach (char x in username)
            {
                if (!char.IsLetterOrDigit(x))
                {
                    error = "Uporabniško ime sme vsebovati samo črke in številke.";
                    return false;
                }
            }

            return true;
        }

        private bool UsernameExists(string username)
        {
            string connStr = ((Site1)Master).GetActiveConnectionString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Uporabnik WHERE [Ime] = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
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