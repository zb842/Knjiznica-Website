<%@ Page Title="Books" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Books.aspx.cs" Inherits="Knjiznica.WebForm2" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-content">
        <h2>Vse knjige</h2>
        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="false" />
    </div>

    <asp:Panel ID="booksContainer" runat="server" CssClass="books-container"></asp:Panel>

</asp:Content>
