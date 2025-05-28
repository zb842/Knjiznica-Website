<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MyBooks.aspx.cs" Inherits="Knjiznica.WebForm3" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-content">

        <h2>Moje knjige</h2>
        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="false"></asp:Label>

    </div>

    <asp:Panel ID="booksContainer" runat="server" CssClass="books-container"></asp:Panel>

</asp:Content>