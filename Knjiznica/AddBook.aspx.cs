using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Knjiznica
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Only admin has access
            string username = Session["User"] as string;
            if (username != "admin")
            {
                Response.Redirect("Books.aspx");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string naslov = txtNaslov.Text.Trim();
                string avtor = txtAvtor.Text.Trim();
                string opis = txtOpis.Text.Trim();

                lblResult.Visible = false;

                //Check for no null input to database
                if (naslov.Length < 1 || naslov.Length > 50 || avtor.Length < 1 || avtor.Length > 50)
                {
                    lblResult.ForeColor = System.Drawing.Color.Red;
                    lblResult.Visible = true;
                    lblResult.Text = "Naslov in avtor morata vsebovati od 1 do 50 znakov.";
                    return;
                }

                string connStr = ((Site1)Master).GetActiveConnectionString();
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    //Check if naslov already exists
                    string checkSql = "SELECT COUNT(*) FROM Knjiga WHERE Naslov = @Naslov";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Naslov", naslov);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblResult.ForeColor = System.Drawing.Color.Red;
                            lblResult.Visible = true;
                            lblResult.Text = "Knjiga s tem naslovom že obstaja.";
                            return;
                        }
                    }

                    //If no image selected take default
                    string imagePath = "/Images/placeholder_book.png";

                    //Check if file selected and if its png or jpg
                    if (fileUploadImage.HasFile)
                    {
                        string ext = System.IO.Path.GetExtension(fileUploadImage.FileName).ToLower();
                        if (ext != ".png" && ext != ".jpg")
                        {
                            lblResult.ForeColor = System.Drawing.Color.Red;
                            lblResult.Visible = true;
                            lblResult.Text = "Prosim izberite sliko v formatu .png ali .jpg";
                            return;
                        }

                        //Save uploaded file to Images folder
                        string fileName = System.IO.Path.GetFileName(fileUploadImage.FileName);
                        string savePath = Server.MapPath("/Images/" + fileName);
                        fileUploadImage.SaveAs(savePath);

                        //Database url standard
                        imagePath = "/Images/" + fileName;
                    }

                    //Database add entry(Knjiga)
                    string sql = "INSERT INTO Knjiga (Naslov, Avtor, Slika, Opis) VALUES (@Naslov, @Avtor, @Slika, @Opis)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Naslov", naslov);
                        cmd.Parameters.AddWithValue("@Avtor", avtor);
                        cmd.Parameters.AddWithValue("@Slika", imagePath);
                        cmd.Parameters.AddWithValue("@Opis", opis);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblResult.ForeColor = System.Drawing.Color.Blue;
                lblResult.Visible = true;
                lblResult.Text = "Knjiga je bila uspešno dodana.";

                //Clear after saving
                txtNaslov.Text = "";
                txtAvtor.Text = "";
                txtOpis.Text = "";
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Pri dodajanju knjige je prišlo do napake: " + ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Books.aspx");
        }
    }
}