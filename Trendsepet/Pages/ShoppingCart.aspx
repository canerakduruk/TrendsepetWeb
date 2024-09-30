<%@ Page Title="" Language="C#" Async="true" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="Trendsepet.Pages.ShoppingCart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shopping-cart-container">
        <h2 class="shopping-cart-header">Sepetim</h2>
        <!-- Başlık -->
        <div class="shopping-cart-content">
            <div class="shopping-cart-product-wrapper">
                <asp:Repeater ID="ProductsRepeater" runat="server" OnItemDataBound="ProductsRepeater_ItemDataBound">
                    <ItemTemplate>
                        <div class="shopping-cart-product-card">
                            <img class="shopping-cart-product-image" src='<%# Eval("imageUrl") %>' alt='<%# Eval("name") %>' />
                            <div class="shopping-cart-product-info">
                                <h5 class="shopping-cart-product-title"><%# Eval("name") %></h5>
                                <asp:Button ID="decreaseBtn" runat="server" CssClass="shopping-cart-quantity-button minus" Text="-"
                                    CommandArgument='<%# Eval("Id") + "," + Eval("Count") %>' OnClick="DecreaseQuantity_Click" />

                                <asp:Label ID="quantityLabel" runat="server" CssClass="shopping-cart-quantity"><%# Eval("Count") %></asp:Label>
                                <asp:Button ID="increaseBtn" runat="server" CssClass="shopping-cart-quantity-button plus" Text="+" OnClick="IncreaseQuantity_Click" CommandArgument='<%# Eval("Id")+","+ Eval("Count") %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="shopping-cart-total-section">
                <h4>Toplam Fiyat: <span class="shopping-cart-total-price">
                    <asp:Label ID="lblTotalPrice" runat="server" Text="$0.00"></asp:Label>
                </span></h4>        
                <button class="shopping-cart-checkout-button">Alışverişi Tamamla</button>
            </div>
        </div>
    </div>
</asp:Content>
