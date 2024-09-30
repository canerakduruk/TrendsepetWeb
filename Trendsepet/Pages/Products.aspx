<%@ Page Title="" Language="C#" Async="true" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="Trendsepet.Pages.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="wrapper d-flex">
        <!-- Sol Kategori Listesi -->
        <div class="category-list me-4">
            <h4>Kategoriler</h4>
            <asp:Repeater ID="MainCategoryRepeater" runat="server">
                <ItemTemplate>
                    <div class="main-category">
                        <a href="javascript:void(0);" class="category-link" onclick="toggleChildCategories(this)">
                            <%# Eval("name") %>
                        </a>
                        <div class="child-category-list" style="display: none;">
                            <asp:Repeater ID="ChildCategoryRepeater" runat="server" DataSource='<%# Eval("ChildCategories") %>'>
                                <ItemTemplate>
                                    <a href='Products.aspx?categoryId=<%# Eval("Id") %>' class="child-category-link">
                                        <%# Eval("name") %>
                                    </a>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>

        <!-- Ürünler -->
        <div class="product-wrapper">
            <div class="product-container">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <a href='ProductDetails.aspx?id=<%# Eval("id") %>' class="card-link">
                            <div class="product-card">
                                <img class="product-card-img-top" src='<%# Eval("imageUrl") %>' alt='<%# Eval("name") %>' />
                                <div class="product-card-body">
                                    <h5 class="product-card-title"><%# Eval("name") %></h5>
                                    <p class="product-card-text">
                                        <strong>Price:</strong> <%# Eval("price", "{0:C}") %>
                                    </p>
                                </div>
                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
