using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using Trendsepet.Classes;
using Trendsepet.Models;

namespace Trendsepet.Pages
{
    public partial class Products : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            FirestoreService firestore = new FirestoreService();

            if (!IsPostBack)
            {
                string categoryId = Request.QueryString["categoryId"];
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var queryPayload = new
                    {
                        structuredQuery = new
                        {
                            from = new[]
                    {
                new { collectionId = "products" }
            },
                            where = new
                            {
                                fieldFilter = new
                                {
                                    field = new { fieldPath = "category" },
                                    op = "EQUAL",
                                    value = new { stringValue = categoryId }
                                }
                            }
                        }
                    };
                    List<JObject> jList = await firestore.GetDocumentsAsync(queryPayload, "products");
                    List<ProductModel> productList = new List<ProductModel>();

                    // Gelen verileri işleme
                    foreach (var content in jList)
                    {
                        var name = content["fields"]["name"]["stringValue"].ToString();
                        var price = int.Parse(content["fields"]["price"]["stringValue"].ToString());
                        var kdvRate = int.Parse(content["fields"]["kdvRate"]["stringValue"].ToString());
                        var imageUrl = content["fields"]["imageUrl"]["stringValue"].ToString();
                        var id = content["name"].ToString().Split('/').Last();
                        var finalPrice = price + (price * (kdvRate / 100));

                        // Yeni bir MainCategoryModel oluştur
                        productList.Add(new ProductModel
                        {
                            Name = name,
                            Id = id,
                            Price = finalPrice.ToString("C"),
                            ImageUrl = imageUrl

                        });
                    }
                    Repeater1.DataSource = productList;
                    Repeater1.DataBind();
                }
                else
                {
                    List<ProductModel> productList = await firestore.GetProductsAsync();
                    Repeater1.DataSource = productList;
                    Repeater1.DataBind();
                }

                try
                {
                    // Kategorileri yükleme
                    var queryBody = new
                    {
                        structuredQuery = new
                        {
                            from = new[] { new { collectionId = "categories_parent" } }
                        }
                    };

                    List<JObject> documentContents = await firestore.GetDocumentsAsync(queryBody, "categories_parent");
                    List<ParentCategoryModel> mainCategoryList = new List<ParentCategoryModel>();

                    // Gelen verileri işleme
                    foreach (var content in documentContents)
                    {
                        var name = content["fields"]["name"]["stringValue"].ToString();
                        var id = content["name"].ToString().Split('/').Last();

                        // Yeni bir MainCategoryModel oluştur
                        mainCategoryList.Add(new ParentCategoryModel
                        {
                            Name = name,
                            Id = id,
                            ChildCategories = await GetChildCategoriesAsync(id)
                        });
                    }

                    MainCategoryRepeater.DataSource = mainCategoryList;
                    MainCategoryRepeater.DataBind();
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                    Response.Write($"Error loading categories: {ex.Message}");
                }


            }
        }

        private async Task<List<ChildCategoryModel>> GetChildCategoriesAsync(string parentId)
        {
            FirestoreService firestore = new FirestoreService();
            var queryBody = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "categories_child" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "parentId" },
                            op = "EQUAL",
                            value = new { stringValue = parentId }
                        }
                    }
                }
            };

            List<JObject> documentContents = await firestore.GetDocumentsAsync(queryBody, "categories_child");
            List<ChildCategoryModel> childCategoryList = new List<ChildCategoryModel>();

            foreach (var content in documentContents)
            {
                var name = content["fields"]["name"]["stringValue"].ToString();
                var id = content["name"].ToString().Split('/').Last();

                childCategoryList.Add(new ChildCategoryModel
                {
                    Name = name,
                    Id = id,
                    ParentId = parentId
                });
            }

            return childCategoryList;
        }
    }
}
