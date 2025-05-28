<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BookDetails.aspx.cs" Inherits="Knjiznica.WebForm6" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="book-detail-container">

        <asp:Button ID="btnNazaj" runat="server" Text="Nazaj" CssClass="bookD-button" OnClick="btnNazaj_Click" />

        <div class="naslov-container">

            <h2 id="lblTitle" runat="server"></h2>
            <asp:TextBox ID="txtNaslov" runat="server" CssClass="textbox textbox-naslov" Visible="false"></asp:TextBox>
        
        </div>

        <div class="book-detail-content">
            
            <asp:Image ID="bookImage" runat="server" CssClass="book-detail-image" ImageUrl="~/Images/placeholder_book.png" />

            <div class="book-detail-info">

                <p><strong>Avtor:</strong> <span id="lblAuthor" runat="server"></span></p>
                <asp:TextBox ID="txtAvtor" runat="server" CssClass="textbox" Visible="false"></asp:TextBox>
                
                <p><strong>Opis:</strong> <span id="lblDescription" runat="server"></span></p>
                <asp:TextBox ID="txtOpis" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="5" Visible="false"></asp:TextBox>

                <asp:FileUpload ID="fileUploadImage" runat="server"  Visible="false" />

            </div>

        </div>

        <asp:Button ID="btnShrani" runat="server" Text="Shrani" CssClass="btn-shrani" Visible="false" OnClick="btnShrani_Click" />

        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="false" />

    </div>

</asp:Content>
