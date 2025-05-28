using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Knjiznica
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null || string.IsNullOrEmpty(Session["User"].ToString()))
            {
                Response.Redirect("Prijava.aspx");
                return;
            }

            //For BookDetails-Back button session check
            Session["PreviousPage"] = "MyBooks";

            //Refresh owned books
            LoadUserBooks();

        }

        private void LoadUserBooks()
        {
            try
            {
                string username = Session["User"] as string;

                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    int userId = 0;
                    using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Uporabnik WHERE Ime = @username", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            Response.Redirect("Home.aspx");
                            return;
                        }
                        userId = Convert.ToInt32(result);

                        //Session userId for button remove
                        Session["userId"] = userId;
                    }

                    //Query user books
                    string sql = @"
                    SELECT K.ID, K.Naslov, K.Avtor, K.Slika
                    FROM Nakup N
                    INNER JOIN Knjiga K ON N.KnjigaID = K.ID
                    WHERE N.UporabnikID = @userId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            booksContainer.Controls.Clear();

                            while (reader.Read())
                            {
                                int bookId = Convert.ToInt32(reader["ID"]);
                                string naslov = reader["Naslov"] as string ?? "";
                                string avtor = reader["Avtor"] as string ?? "";
                                string slika = reader["Slika"] as string ?? "";

                                // Create the full card as a Panel
                                Panel card = new Panel();
                                card.CssClass = "book-card";

                                //Limit char
                                string trimmedTitle = naslov.Length > 20 ? naslov.Substring(0, 20) + "..." : naslov;
                                var title = new HtmlGenericControl("h3");
                                title.InnerText = trimmedTitle;
                                card.Controls.Add(title);

                                //Limit char
                                string trimmedAuthor = avtor.Length > 15 ? avtor.Substring(0, 15) + "..." : avtor;
                                Label authorLabel = new Label();
                                authorLabel.Text = "Avtor: " + trimmedAuthor;
                                card.Controls.Add(authorLabel);

                                //Trim url from database so no empty spaces and placeholder image if null
                                Image bookImage = new Image();
                                string trimmedSlika = slika?.Trim();
                                bookImage.ImageUrl = ResolveUrl(string.IsNullOrEmpty(trimmedSlika) ? "~/images/placeholder_book.png" : trimmedSlika);
                                bookImage.CssClass = "book-image";
                                card.Controls.Add(bookImage);

                                //Redirect to different page
                                Button btnInfo = new Button();
                                btnInfo.Text = "Info";
                                btnInfo.CssClass = "book-button";
                                btnInfo.PostBackUrl = "BookDetails.aspx?bookId=" + bookId;
                                card.Controls.Add(btnInfo);

                                //Remove book from user
                                Button btnOdstrani = new Button();
                                btnOdstrani.Text = "Odstrani";
                                btnOdstrani.CssClass = "book-button";
                                btnOdstrani.CommandArgument = bookId.ToString();
                                btnOdstrani.Click += BtnOdstrani_Click;
                                card.Controls.Add(btnOdstrani);

                                //Add cards
                                booksContainer.Controls.Add(card);
                            }
                        }
                    }

                    // Add "Dodaj novo" card
                    Panel addNewCard = new Panel();
                    addNewCard.CssClass = "book-card add-new-card";
                    addNewCard.Attributes["onclick"] = "location.href='Books.aspx';";
                    addNewCard.Style["cursor"] = "pointer";

                    var addTitle = new HtmlGenericControl("h3");
                    addTitle.InnerText = "Dodaj novo";
                    addNewCard.Controls.Add(addTitle);

                    Image addImage = new Image();
                    addImage.ImageUrl = ResolveUrl("~/images/add_book.jpg");
                    addImage.CssClass = "book-image";
                    addNewCard.Controls.Add(addImage);

                    booksContainer.Controls.Add(addNewCard);
                }
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Pri nalaganju knjig je prišlo do napake. Poskusite ponovno kasneje: " + ex.Message;
            }
        }

        protected void BtnOdstrani_Click(object sender, EventArgs e)
        {
            int userId = (int)Session["UserId"];
            Button btn = (Button)sender;
            int bookId = Convert.ToInt32(btn.CommandArgument);

            string connStr = ((Site1)Master).GetActiveConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Nakup WHERE UporabnikID = @userId AND KnjigaID = @bookId", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@userId", userId);
                        deleteCmd.Parameters.AddWithValue("@bookId", bookId);

                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblResult.ForeColor = System.Drawing.Color.Blue;
                            lblResult.Visible = true;
                            lblResult.Text = "Knjiga je bila odstranjena.";
                        }
                        else
                        {
                            lblResult.ForeColor = System.Drawing.Color.Red;
                            lblResult.Visible = true;
                            lblResult.Text = "Knjiga ni bila najdena.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri odstranjevanju knjige: " + ex.Message;
            }

            //Reload page so removed books are not there
            Response.Redirect(Request.RawUrl);
        }
    }
}