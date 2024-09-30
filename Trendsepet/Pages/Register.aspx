<%@ Page Title="" Language="C#" MasterPageFile="~/Layout.Master" Async="true" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Trendsepet.Pages.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <h3>Kayıt</h3>

        <div class="form-group">
            <label for="email">Email address:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="pwd">Password:</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>
        <asp:Label ID="lblMessage" runat="server" Text="Initial Text"></asp:Label>


        <asp:Button ID="btnRegister" runat="server" CssClass="btn btn-primary" Text="Kayıt Ol" OnClick="btnRegister_Click"  />



</asp:Content>
