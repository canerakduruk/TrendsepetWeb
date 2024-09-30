using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Trendsepet.Classes;
using Trendsepet.Models;

namespace Trendsepet.Pages
{
    public partial class ShoppingCart : System.Web.UI.Page
    {
        FirestoreService firestoreService;
        List<ProductModel> productList;
        int totalPrice = 0;


        protected async void Page_Load(object sender, EventArgs e)
        {
            firestoreService = new FirestoreService();
            if (!IsPostBack)
            {
                await GetCart();
            }
        }

        protected void ProductsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblQuantity = (Label)e.Item.FindControl("quantityLabel");
                if (lblQuantity != null)
                {
                    // DataBinder ile Eval kullanarak veriyi al
                    string quantity = DataBinder.Eval(e.Item.DataItem, "Count").ToString();
                    lblQuantity.Text = quantity;
                    Response.Write($"Quantity: {quantity}"); // Test amaçlı yazdır
                }
            }
        }

        protected async void IncreaseQuantity_Click(object sender, EventArgs e)
        {
            var button = (Button)sender; // Tıklanan buton
            var itemContainer = (RepeaterItem)button.NamingContainer;
            Label lblQuantity = (Label)itemContainer.FindControl("quantityLabel");
            string productId = button.CommandArgument.Split(',')[0];
            var cart = new CartModel
            {
                ProductId = productId,
                CustomerId = "7V73gvMGB1UCBTTrkhJxqXNXBdJ2"
            };

            var jsonBody = new
            {
                fields = new
                {
                    productId = new { stringValue = cart.ProductId },
                    customerId = new { stringValue = cart.CustomerId },
                }
            };

            var result = await firestoreService.AddDataAsync(jsonBody,"shopping_cart");

            if (lblQuantity != null)
            {
                // Mevcut miktarı al ve artır
                int currentCount = int.Parse(lblQuantity.Text);
                currentCount++;
                lblQuantity.Text = currentCount.ToString(); // Yeni miktarı güncelle

                // Firestore'a yeni miktarı kaydet





                if (!result)
                {
                    Response.Write("Kaydetme işlemi sırasında bir hata oluştu.");
                }
            }
        }


        protected async void DecreaseQuantity_Click(object sender, EventArgs e)
        {




            var button = (Button)sender; // Tıklanan buton
            var itemContainer = (RepeaterItem)button.NamingContainer;
            Label lblQuantity = (Label)itemContainer.FindControl("quantityLabel");
            string productId = button.CommandArgument.Split(',')[0];
            var cart = new CartModel
            {
                ProductId = productId,
                CustomerId = "7V73gvMGB1UCBTTrkhJxqXNXBdJ2"
            };

            var queryBody = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "shopping_cart" } }, // Koleksiyon adı
                    where = new
                    {
                        compositeFilter = new
                        {
                            op = "AND",
                            filters = new[]
                            {
                        new {
                            fieldFilter = new
                            {
                                field = new { fieldPath = "productId" },
                                op = "EQUAL",
                                value = new { stringValue = productId }
                            }
                        },
                        new {
                            fieldFilter = new
                            {
                                field = new { fieldPath = "customerId" },
                                op = "EQUAL",
                                value = new { stringValue = "7V73gvMGB1UCBTTrkhJxqXNXBdJ2" }
                            }
                        }
                    }
                        }
                    },
                    limit = 1
                }
            };

            await firestoreService.DeleteDocumentAsync(queryBody, "shopping_cart");

            if (lblQuantity != null)
            {
                // Mevcut miktarı al ve artır
                int currentCount = int.Parse(lblQuantity.Text);
                currentCount--;
                lblQuantity.Text = currentCount.ToString(); // Yeni miktarı güncelle

                // Firestore'a yeni miktarı kaydet
                if (currentCount < 1)
                {
                    Response.Redirect(Request.RawUrl);
                }


            }

        }

        private async Task GetCart()
        {
            try
            {
                var firestoreService = new FirestoreService();
                List<string> idList = await firestoreService.GetProductIdsByCustomer("7V73gvMGB1UCBTTrkhJxqXNXBdJ2");
                var tasks = new List<Task>();

                if (idList.Count > 0)
                {
                    productList = await firestoreService.GetProductsByIds(idList);

                    foreach (ProductModel productModel in productList)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            int count = await firestoreService.GetProductCountByCart(productModel.Id, "7V73gvMGB1UCBTTrkhJxqXNXBdJ2");
                            productModel.Count = count.ToString(); // Miktar bilgisini string'e çeviriyoruz
                            totalPrice += int.Parse(productModel.Price)*count;
                        }));
                    }

                    await Task.WhenAll(tasks);

                    lblTotalPrice.Text = totalPrice.ToString();
                    ProductsRepeater.DataSource = productList;
                    ProductsRepeater.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error: {ex.Message}");
            }
        }
    }
}
