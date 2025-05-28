<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Prijava.aspx.cs" Inherits="Knjiznica.WebForm4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="centered-form-container">

        <h2>Prijavite se</h2>
        
        <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox" placeholder="Uporabniško ime"></asp:TextBox>
        <br />
        
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="textbox" placeholder="Geslo"></asp:TextBox>
        <br />
        
        <asp:Button ID="Prijava" runat="server" Text="Prijava" OnClick="Prijava_Click" CssClass="btn-submit" />
        <br />
        
        <asp:Label ID="lblResult" runat="server" Text="" ForeColor="Red" />

    </div>

</asp:Content>
