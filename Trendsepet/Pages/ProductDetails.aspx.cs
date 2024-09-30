using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Trendsepet.Classes;
using Trendsepet.Models;

namespace Trendsepet.Pages
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        ProductModel product;
        FirestoreService firestoreService;

        protected async void Page_Load(object sender, EventArgs e)
        {
            firestoreService = new FirestoreService();

            if (!IsPostBack)
            {
                string productId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(productId))
                {
                    product = await firestoreService.GetProductById(productId);
                    ViewState["Product"] = product;

                    // Product nesnesinin null olup olmadığını kontrol et
                    if (product != null)
                    {
                        lblProductName.Text = product.Name;
                        lblProductDescription.Text = product.Description;
                        lblProductPrice.Text = product.Price; // Fiyatı para formatında göster
                        imgProduct.ImageUrl = product.ImageUrl;

                        // Kullanıcı ID'sini al
                        string token = firestoreService.GetAuthToken();
                        string userId = await firestoreService.GetUserIdFromToken(token);

                        // Ürünün sepetteki miktarını al
                        int count = await firestoreService.GetProductCountByCart(productId, userId);
                        lblCount.Text = count.ToString();
                    }
                    else
                    {
                        lblProductName.Text = "Ürün bulunamadı.";
                        lblProductDescription.Text = "";
                        lblProductPrice.Text = "";
                        imgProduct.ImageUrl = ""; // Resmi temizle
                    }
                }
                else
                {
                    lblProductName.Text = "Ürün ID'si geçersiz.";
                }
            }
        }

        protected async void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Product nesnesinin null olup olmadığını kontrol et
            product = ViewState["Product"] as ProductModel;
            Response.Write(product.Id);

            if (product != null)
            {
                var jsonBody = new
                {
                    fields = new
                    {
                        productId = new { stringValue = product.Id },
                        customerId = new { stringValue = "7V73gvMGB1UCBTTrkhJxqXNXBdJ2" },
                    }
                };

                var isSuccessful = await firestoreService.AddDataAsync(jsonBody, "shopping_cart");
                if (isSuccessful)
                {
                    Response.Write("Başarılı");
                    Response.Redirect("ShoppingCart.aspx");
                }
                else
                {
                    Response.Write("Başarısız");
                }
            }
            else
            {
                Response.Write("Ürün sepete eklenemedi, ürün bilgisi bulunamadı.");
            }
        }
    }
}
