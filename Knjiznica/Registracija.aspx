<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Registracija.aspx.cs" Inherits="Knjiznica.WebForm5" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="centered-form-container">

        <h2>Registrirajte se</h2>
        
        <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox" placeholder="Uporabniško ime"></asp:TextBox>
        <br />
        
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="textbox" placeholder="Geslo"></asp:TextBox>
        <br />

        <asp:Button ID="Registracija" runat="server" Text="Registracija" OnClick="Registracija_Click" CssClass="btn-submit" />
        <br />
        
        <asp:Label ID="lblResult" runat="server" ForeColor="Red"></asp:Label>

    </div>

</asp:Content>
