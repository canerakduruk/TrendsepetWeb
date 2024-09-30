<%@ Page Language="C#" MasterPageFile="~/Layout.Master" Async="true" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Trendsepet.Pages.Login" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


        <h3>Giriş</h3>

        <div class="form-group">
            <label for="email">Email address:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="pwd">Password:</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>

        <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary" Text="Giriş" OnClick="btnLogin_Click" />



</asp:Content>

