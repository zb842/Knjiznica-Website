<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Knjiznica.Home" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="page-content">
        <h2>Dobrodošli na E-knjižnici</h2>
        <p>Dobrodošli na E-knjižnici. Tukaj boste našli široko zbirko knjig v elektronski obliki, ki jih lahko berete kjerkoli in kadarkoli. Uživajte v branju.</p>
    
    </div>

    <div class="page-content">
        
        <h2>Trenutno najbolj priljubljena knjiga:</h2>

    </div>

    <div class="picture-zone">

        <asp:Label ID="lblTopBookTitle" runat="server" CssClass="book-title" Text=""></asp:Label>
        <br />

        <asp:Image ID="bookImage" runat="server" CssClass="book-image" 
            ImageUrl="~/images/placeholder_book.png" AlternateText="Najbolj priljubljena trenutno" />

    </div>

    <div class="page-content">

        <h2>Če še nisi prijavljen, se <a href="Registracija.aspx">registriraj tukaj</a></h2>
        <h2>Kratka navodila za uporabo:</h2>
        <p>Najprej se <a href="Prijava.aspx">prijaviš</a> ali <a href="Registracija.aspx">registriraš</a>, nato lahko na strani <a href="MyBooks.aspx">Moje knjige</a> vidiš svojo zbirko ali pa si na <a href="Books.aspx">Knjige</a> izbereš nove.</p>
    
    </div>

</asp:Content>