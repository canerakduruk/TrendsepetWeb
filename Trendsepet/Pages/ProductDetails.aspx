<%@ Page Title="" Language="C#" Async="true" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="Trendsepet.Pages.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="product-detail-container" style="margin-top: 20px">
        <div class="product-detail-image-card">
            <asp:Image ID="imgProduct" runat="server" alt="Ürün Resmi" CssClass="product-detail-image" />
        </div>
        <div class="product-detail-info" >
            <asp:Label ID="lblProductName" runat="server" CssClass="product-detail-name"></asp:Label>
            <asp:Label ID="lblProductDescription" runat="server" CssClass="product-detail-description"></asp:Label>
            <p>
                <strong>Fiyat: </strong>
                <asp:Label ID="lblProductPrice" runat="server" CssClass="product-detail-price"></asp:Label>
            </p>
            <asp:Label ID="lblCount" runat="server"></asp:Label>
            <asp:Button ID="btnAddToCart" runat="server" Text="Sepete Ekle" OnClick="btnAddToCart_Click" CssClass="btn btn-primary" />
        </div>
    </div>
</asp:Content>
