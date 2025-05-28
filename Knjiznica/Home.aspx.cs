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
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadPopularBookImage();
        }
        private void LoadPopularBookImage()
        {
            try
            {
                string connStr = ((Site1)Master).GetActiveConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"
                    SELECT TOP 1 K.Slika, K.Naslov
                    FROM Nakup N
                    INNER JOIN Knjiga K ON N.KnjigaID = K.ID
                    WHERE K.Slika IS NOT NULL
                      AND K.Slika <> ''
                      AND K.Slika NOT LIKE '%placeholder_book.png%'
                    GROUP BY K.ID, K.Slika, K.Naslov
                    ORDER BY COUNT(*) DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTopBookTitle.Text = reader["Naslov"] as string ?? "";
                                string slika = reader["Slika"] as string ?? "";

                                string trimmedSlika = slika?.Trim();
                                bookImage.ImageUrl = ResolveUrl(string.IsNullOrEmpty(trimmedSlika) ? "~/images/placeholder_book.png" : trimmedSlika);
                            }
                            else
                            {

                                bookImage.ImageUrl = "~/images/placeholder_book.png";
                                lblTopBookTitle.Text = "Ni dovolj podatkov";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bookImage.ImageUrl = "~/images/placeholder_book.png";
                lblTopBookTitle.Text = $"{ex}";
            }
        }
    }
}