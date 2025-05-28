<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AddBook.aspx.cs" Inherits="Knjiznica.WebForm7" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="page-content">

        <h2>Dodaj novo knjigo</h2>

    </div>

    <div class="addbook-form-container">

        <asp:Label ID="lblResult" runat="server" Text="" ForeColor="Red" Visible="false" />
        <br />

        <asp:Label ID="lblNaslov" runat="server" Text="Naslov:" AssociatedControlID="txtNaslov"></asp:Label>
        <asp:TextBox ID="txtNaslov" runat="server" CssClass="textbox"></asp:TextBox>

        <asp:Label ID="lblAvtor" runat="server" Text="Avtor:" AssociatedControlID="txtAvtor"></asp:Label>
        <asp:TextBox ID="txtAvtor" runat="server" CssClass="textbox"></asp:TextBox>

        <asp:Label ID="lblSlika" runat="server" Text="Izberi sliko (.png):"></asp:Label>
        <asp:FileUpload ID="fileUploadImage" runat="server" />

        <asp:Label ID="lblOpis" runat="server" Text="Opis:" AssociatedControlID="txtOpis"></asp:Label>
        <asp:TextBox ID="txtOpis" runat="server" TextMode="MultiLine" Rows="5" CssClass="textbox"></asp:TextBox>

        <asp:Button ID="btnSubmit" runat="server" Text="Shrani knjigo" CssClass="btn-submit" OnClick="btnSubmit_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Prekliči" CssClass="btn-submit" OnClick="btnCancel_Click" />
    
    </div>
</asp:Content>

