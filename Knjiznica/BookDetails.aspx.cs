using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Knjiznica
{
    public partial class WebForm6 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string bookIdStr = Request.QueryString["bookId"];
            if (string.IsNullOrEmpty(bookIdStr) || !int.TryParse(bookIdStr, out int bookId))
            {
                RedirectBack();
                return;
            }

            Session["EditBookId"] = bookId;

            if (!IsPostBack)
            {
                LoadBookDetails(bookId);
            }

            if (Session["User"] != null && Session["User"].ToString() == "admin")
            {

                txtNaslov.Visible = true;
                txtAvtor.Visible = true;
                txtOpis.Visible = true;
                fileUploadImage.Visible = true;
                btnShrani.Visible = true;

                lblTitle.Visible = !true;
                lblAuthor.Visible = !true;
                lblDescription.Visible = !true;
            }
            else
            {
                txtNaslov.Visible = false;
                txtAvtor.Visible = false;
                txtOpis.Visible = false;
                fileUploadImage.Visible = false;
                btnShrani.Visible = false;

                lblTitle.Visible = !false;
                lblAuthor.Visible = !false;
                lblDescription.Visible = !false;
            }
        }

        private void LoadBookDetails(int bookId)
        {
            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Naslov, Avtor, Slika, Opis FROM Knjiga WHERE ID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", bookId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Admin editing
                                string naslov = reader["Naslov"] as string ?? "";
                                string avtor = reader["Avtor"] as string ?? "";
                                string opis = reader["Opis"] as string ?? "";

                                txtNaslov.Text = naslov;
                                txtAvtor.Text = avtor;
                                txtOpis.Text = opis;
                                
                                //User view
                                lblTitle.InnerText = reader["Naslov"] as string ?? "";
                                lblAuthor.InnerText = reader["Avtor"] as string ?? "";
                                lblDescription.InnerText = reader["Opis"] as string ?? "";
                                string slika = reader["Slika"] as string ?? "";

                                string trimmedSlika = slika?.Trim();
                                bookImage.ImageUrl = ResolveUrl(string.IsNullOrEmpty(trimmedSlika) ? "~/images/placeholder_book.png" : trimmedSlika);
                            }
                            else
                            {
                                Session["ErrorMessage"] = "Knjiga ne obstaja. Poskusite ponovno.<br />";
                                RedirectBack();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Session["ErrorMessage"] = "Pri nalaganju knjige je prišlo do napake.<br />" + ex.Message;
                RedirectBack();
            }
        }

        protected void btnNazaj_Click(object sender, EventArgs e)
        {
            RedirectBack();
        }

        protected void RedirectBack()
        {
            string previousPage = Session["PreviousPage"] as string;

            if (!string.IsNullOrEmpty(previousPage))
            {
                Response.Redirect(previousPage + ".aspx");
            }
            else
            {
                Response.Redirect("Books.aspx");
            }
        }

        protected void btnShrani_Click(object sender, EventArgs e)
        {
            try
            {
                string naslov = txtNaslov.Text.Trim();
                string avtor = txtAvtor.Text.Trim();
                string opis = txtOpis.Text.Trim();
                string slika = bookImage.ImageUrl;

                string imagePath = slika;

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

                    //Check if naslov already exists, exclude current edit book
                    string checkSql = "SELECT COUNT(*) FROM Knjiga WHERE Naslov = @Naslov AND ID <> @Id";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Naslov", naslov);
                        checkCmd.Parameters.AddWithValue("@Id", (int)Session["EditBookId"]);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblResult.ForeColor = System.Drawing.Color.Red;
                            lblResult.Visible = true;
                            lblResult.Text = "Knjiga s tem naslovom že obstaja.";
                            return;
                        }
                    }

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


                    //Database edit entry(Knjiga)
                    string sql = "UPDATE Knjiga SET Naslov = @Naslov, Avtor = @Avtor, Opis = @Opis, Slika = @Slika WHERE ID = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Naslov", naslov);
                        cmd.Parameters.AddWithValue("@Avtor", avtor);
                        cmd.Parameters.AddWithValue("@Opis", opis);
                        cmd.Parameters.AddWithValue("@Slika", imagePath);
                        cmd.Parameters.AddWithValue("@Id", (int)Session["EditBookId"]);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblResult.ForeColor = System.Drawing.Color.Blue;
                lblResult.Visible = true;
                lblResult.Text = "Podatki uspešno spremenjeni.";

                //Reload page info
                LoadBookDetails((int)Session["EditBookId"]);
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri shranjevanju: " + ex.Message;
            }
        }

    }
}