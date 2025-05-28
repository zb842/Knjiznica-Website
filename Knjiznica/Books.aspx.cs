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
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //For BookDetails-Back button session check
            Session["PreviousPage"] = "Books";

            //Refresh owned books
            LoadAllBooks();
        }

        private void LoadAllBooks()
        {
            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT ID, Naslov, Avtor, Slika FROM Knjiga";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        booksContainer.Controls.Clear();

                        string currentUser = Session["User"] as string;

                        while (reader.Read())
                        {
                            int bookId = Convert.ToInt32(reader["ID"]);
                            string naslov = reader["Naslov"] as string ?? "";
                            string avtor = reader["Avtor"] as string ?? "";
                            string slika = reader["Slika"] as string ?? "";

                            //Book card
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

                            //Database adding(nakup)
                            Button btnAdd = new Button();
                            btnAdd.Text = "Dodaj";
                            btnAdd.CommandArgument = bookId.ToString();
                            btnAdd.CssClass = "book-button";
                            btnAdd.Click += BtnDodaj_Click;
                            card.Controls.Add(btnAdd);

                            if (currentUser == "admin")
                            {
                                //Add extra button for delete for admin
                                Button btnDelete = new Button();
                                btnDelete.Text = "Izbriši";
                                btnDelete.CommandArgument = bookId.ToString();
                                btnDelete.CssClass = "book-button";
                                btnDelete.Click += BtnIzbrisi_Click;
                                card.Controls.Add(btnDelete);
                            }
                            //No user
                            if (string.IsNullOrEmpty(currentUser))
                            {
                                booksContainer.Controls.Add(card);
                            }
                            //Admin
                            else if (currentUser == "admin")
                            {
                                booksContainer.Controls.Add(card);
                            }
                            //User
                            else
                            {
                                if (string.IsNullOrEmpty(currentUser) || !UserBook(conn, currentUser, bookId))
                                {
                                    booksContainer.Controls.Add(card);
                                }
                            }
                        }
                    }

                    //Add newbook card for admin
                    if (Session["User"] != null && Session["User"].ToString() == "admin")
                    {
                        Panel addNewCard = new Panel();
                        addNewCard.CssClass = "book-card add-new-card";
                        addNewCard.Attributes["onclick"] = "location.href='AddBook.aspx';";
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
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri nalaganju knjig: " + ex.Message;
            }
        }

        private bool UserBook(SqlConnection conn, string username, int bookId)
        {
            try
            {
                int userId;

                using (SqlCommand getUserCmd = new SqlCommand("SELECT ID FROM Uporabnik WHERE Ime = @username", conn))
                {
                    getUserCmd.Parameters.AddWithValue("@username", username);
                    object result = getUserCmd.ExecuteScalar();

                    //If user not found in database
                    if (result == null)
                    {
                        lblResult.ForeColor = System.Drawing.Color.Red;
                        lblResult.Visible = true;
                        lblResult.Text = "Uporabnik ni bil najden. Še enkrat se prijavi.";
                        return false;
                    }
                    userId = Convert.ToInt32(result);
                }

                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Nakup WHERE UporabnikID = @userId AND KnjigaID = @bookId", conn))
                {
                    checkCmd.Parameters.AddWithValue("@userId", userId);
                    checkCmd.Parameters.AddWithValue("@bookId", bookId);
                    int count = (int)checkCmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri preverjanju knjig uporabnika: " + ex.Message;
                return true;
            }
        }

        protected void BtnDodaj_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int bookId = Convert.ToInt32(btn.CommandArgument);

            if (Session["User"] == null || string.IsNullOrEmpty(Session["User"].ToString()))
            {
                Response.Redirect("Prijava.aspx");
                return;
            }

            string username = Session["User"].ToString();

            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    //User check
                    int userId;
                    using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Uporabnik WHERE Ime = @username", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            Response.Redirect("Prijava.aspx");
                            return;
                        }
                        userId = Convert.ToInt32(result);
                    }

                    //UserBook check no duplication
                    using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Nakup WHERE UporabnikID = @userId AND KnjigaID = @bookId", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", userId);
                        checkCmd.Parameters.AddWithValue("@bookId", bookId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblResult.ForeColor = System.Drawing.Color.Blue;
                            lblResult.Visible = true;
                            lblResult.Text = "Knjiga je že v vaši zbirki.";
                            return;
                        }
                    }

                    //Add book to database(nakup)
                    using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Nakup (UporabnikID, KnjigaID, DatumDodano) VALUES (@userId, @bookId, @datum)", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@userId", userId);
                        insertCmd.Parameters.AddWithValue("@bookId", bookId);
                        insertCmd.Parameters.AddWithValue("@datum", DateTime.Now);
                        insertCmd.ExecuteNonQuery();
                    }
                }

                //Reload page so added books are not there
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri dodajanju knjige uporabniku: " + ex.Message;
            }
        }

        protected void BtnIzbrisi_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int bookId = Convert.ToInt32(btn.CommandArgument);

            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Knjiga WHERE ID = @bookId", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@bookId", bookId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    lblResult.ForeColor = System.Drawing.Color.Blue;
                    lblResult.Visible = true;
                    lblResult.Text = "Knjiga uspešno izbrisana.";

                    //Reload page so added books are not there
                    Response.Redirect(Request.RawUrl);
                }
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true;
                lblResult.Text = "Napaka pri brisanju knjige: " + ex.Message;
            }
        }
    }
}