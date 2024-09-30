using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Trendsepet.Models;
using FirebaseAdmin.Auth;
using static Google.Apis.Requests.BatchRequest;
using System.Web;
using System.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Trendsepet.Pages;
using Newtonsoft.Json.Linq;

namespace Trendsepet.Classes
{
    public class FirestoreService
    {
        private readonly RestClient client;
        private readonly string baseUrl = "https://firestore.googleapis.com/v1/projects/trendsepet-c6453/databases/(default)/documents";
        private readonly string _logAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBRRfBteYwSNLiVkHYxj10MQx6g2IveoSU";
        private readonly string _regAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyBRRfBteYwSNLiVkHYxj10MQx6g2IveoSU";
        private readonly string _signOutUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signOut?key=AIzaSyBRRfBteYwSNLiVkHYxj10MQx6g2IveoSU";
        private readonly string projectId = "trendsepet-c6453";
        private readonly string apiKey = "AIzaSyBRRfBteYwSNLiVkHYxj10MQx6g2IveoSU";

        public FirestoreService()
        {
            client = new RestClient(baseUrl);
        }


        //Oturum işlemleri
        public string GetAuthToken()
        {
            var authCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (authCookie != null)
            {
                return authCookie.Value; // Çerezdeki token'ı döner
            }
            return null; // Çerez yoksa null döner
        }
        public async Task<string> GetUserIdFromToken(string token)
        {
            var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

            var requestData = new
            {
                idToken = token
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                return result.users[0].localId; // Kullanıcı ID'sini döner
            }
            else
            {
                // Hata durumunda gerekli işlemleri yapın
                Console.WriteLine($"Token doğrulama hatası: {response.StatusCode}");
                return null;
            }
        }
        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var requestData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            // Yeni bir RestRequest oluşturun ve POST methodunu kullanın
            var request = new RestRequest(_logAuthUrl, Method.Post);

            // Başlıkları ekleyin
            request.AddHeader("Content-Type", "application/json");

            // JSON verisini ekleyin
            request.AddJsonBody(JsonConvert.SerializeObject(requestData));

            // İsteği gönderin
            var response = await client.ExecuteAsync(request);

            // Yanıtı kontrol edin
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string idToken = result.idToken;

                // Çerezi saklayın
                var authCookie = new HttpCookie("AuthToken", idToken)
                {
                    HttpOnly = true, // Çerezin JavaScript tarafından erişilmemesini sağlar
                    Secure = true,   // HTTPS üzerinden gönderildiğinden emin olun
                    Expires = DateTime.Now.AddHours(1) // Çerezin geçerlilik süresi (1 saat)
                };
                HttpContext.Current.Response.Cookies.Add(authCookie);

                return result.idToken;
            }
            else
            {
                throw new Exception($"Error: {response.Content}");
            }


        }
        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password)
        {
            var requestData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var request = new RestRequest(_regAuthUrl, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return result.idToken; // Başarıyla kaydedilen kullanıcının token'ı
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string errorMessage = errorResponse.error.message;
                throw new Exception($"Error: {errorMessage}");
            }
        }

        //Firestore işlemleri
        public async Task<List<ParentCategoryModel>> GetParentCategoriesAsync()
        {
            var request = new RestRequest("https://firestore.googleapis.com/v1/projects/trendsepet-c6453/databases/(default)/documents/categories_parent", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var parentCategories = new List<ParentCategoryModel>();

                foreach (var document in result.documents)
                {
                    ParentCategoryModel category = new ParentCategoryModel();
                    var fields = document.fields;

                    var nameField = fields.name;
                    var idField = document.name.Split('/').Last();

                    category.Name = (string)nameField.stringValue;
                    category.Id = idField;

                    parentCategories.Add(category);
                }

                return parentCategories;
            }
            else
            {
                throw new Exception($"Error: {response.StatusDescription}");
            }
        }

        public async Task<List<ChildCategoryModel>> GetChildCategoriesAsync(string parentId)
        {
            var request = new RestRequest($"https://firestore.googleapis.com/v1/projects/trendsepet-c6453/databases/(default)/documents/categories_child", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var childCategories = new List<ChildCategoryModel>();

                foreach (var document in result.documents)
                {
                    var fields = document.fields;
                    var parentIdField = fields.parentId.stringValue;

                    if (parentIdField == parentId)
                    {
                        ChildCategoryModel category = new ChildCategoryModel();
                        category.Name = fields.name.stringValue;
                        category.Id = document.name.Split('/').Last();

                        childCategories.Add(category);
                    }
                }

                return childCategories;
            }
            else
            {
                throw new Exception($"Error: {response.StatusDescription}");
            }
        }

        public async Task<List<ProductModel>> GetProductsAsync()
        {

            var request = new RestRequest("/products", Method.Get);
            var response = await client.ExecuteAsync(request);



            if (response.IsSuccessful)
            {
                var content = response.Content;
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var productInfo = new List<ProductModel>();

                foreach (var document in result.documents)
                {
                    ProductModel product = new ProductModel();
                    var fields = document.fields;

                    // ID'yi almak için string'e dönüştürme
                    var documentName = (string)document.name; // JValue'den string'e dönüştür
                    var id = documentName.Split('/').Last(); // Son elemanı al

                    product.Id = id; // Eğer ProductModel'de bir Id alanı varsa

                    // Alanları kontrol etmeden al
                    var nameField = fields.name;
                    var priceField = fields.price;
                    var imageField = fields.imageUrl;

                    // `nameField` ve `nameField.stringValue` kontrolü
                    if (nameField != null && nameField.stringValue != null)
                    {
                        product.Name = nameField.stringValue;
                    }

                    if (priceField != null && priceField.stringValue != null)
                    {
                        product.Price = priceField.stringValue; // Fiyat alanını kontrol et
                    }

                    if (imageField != null && imageField.stringValue != null)
                    {
                        product.ImageUrl = imageField.stringValue; // Resim URL'sini kontrol et
                    }

                    productInfo.Add(product);
                }

                return productInfo;
            }
            else
            {
                throw new Exception($"Error: {response.StatusDescription}");
            }
        }


        public async Task<ProductModel> GetProductById(string productId)
        {
            var request = new RestRequest($"products/{productId}", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var product = new ProductModel();

                // Alanları kontrol et
                var fields = result.fields;

                product.Id = productId; // Ürün ID'si
                product.Name = fields.name?.stringValue;
                product.Price = fields.price?.stringValue; // stringValue'dan al
                product.ImageUrl = fields.imageUrl?.stringValue;
                product.Description = fields.description?.stringValue; // Açıklama alanını ekle

                // property alanını harita olarak almak isterseniz
                if (fields.property?.mapValue != null)
                {
                    var propertyFields = fields.property.mapValue.fields;
                    // propertyFields üzerinde işlem yapabilirsiniz
                }

                return product;
            }
            else
            {
                throw new Exception($"Error: {response.StatusDescription}");
            }
        }

        public async Task<int> GetProductCountByCart(string productId, string customerId)
        {
            var request = new RestRequest($"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery?key={apiKey}", Method.Post);

            var queryPayload = new
            {
                structuredQuery = new
                {
                    from = new[]
                    {
                new { collectionId = "shopping_cart" }
            },
                    where = new
                    {
                        compositeFilter = new
                        {
                            op = "AND",
                            filters = new[]
                            {
                        new
                        {
                            fieldFilter = new
                            {
                                field = new { fieldPath = "customerId" },
                                op = "EQUAL",
                                value = new { stringValue = customerId }
                            }
                        },
                        new
                        {
                            fieldFilter = new
                            {
                                field = new { fieldPath = "productId" },
                                op = "EQUAL",
                                value = new { stringValue = productId }
                            }
                        }
                    }
                        }
                    }
                }
            };

            request.AddJsonBody(queryPayload);
            var response = await client.ExecuteAsync(request);


            // Yanıtı işleme
            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);

                // Sadece belge sayısını döndür
                return jsonResponse.Count(document => document.ContainsKey("document"));
            }
            else
            {
                Console.WriteLine($"Veri çekme başarısız oldu: {response.StatusDescription}, Hata kodu: {response.StatusCode}");
                return 0; // Başarısız olduğunda 0 döndür
            }
        }

        public async Task<List<string>> GetProductIdsByCustomer(string customerId)
        {
            var request = new RestRequest($"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery?key={apiKey}", Method.Post);

            var queryPayload = new
            {
                structuredQuery = new
                {
                    from = new[]
                    {
                new { collectionId = "shopping_cart" }
            },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "customerId" },
                            op = "EQUAL",
                            value = new { stringValue = customerId }
                        }
                    }
                }
            };

            request.AddJsonBody(queryPayload);
            var response = await client.ExecuteAsync(request);

            // Yanıtı işleme
            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);

                // customerId eşleşenlerin productId listesi döndürülüyor ve Distinct ile tekrar eden productId'ler filtreleniyor
                return jsonResponse
                    .Where(document => document.ContainsKey("document") && document["document"].ContainsKey("fields"))
                    .Select(document => (string)document["document"]["fields"]["productId"]["stringValue"])
                    .Distinct() // Tekrar eden productId'leri filtreler
                    .ToList();
            }
            else
            {
                Console.WriteLine($"Veri çekme başarısız oldu: {response.StatusDescription}, Hata kodu: {response.StatusCode}");
                return new List<string>(); // Başarısız olduğunda boş bir liste döndür
            }
        }

        public async Task<List<ProductModel>> GetProductsByIds(List<string> productIds)
        {
            var client = new RestClient();
            var collectionId = "products";
            var authToken = HttpContext.Current.Request.Cookies["AuthToken"].Value;  // Firebase Auth Token

            var productList = new List<ProductModel>();

            var tasks = productIds.Select(async documentId =>
            {
                var request = new RestRequest($"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}/{documentId}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {authToken}");
                var response = await client.ExecuteAsync(request);  // Asenkron çağrı

                if (response.IsSuccessful)
                {
                    var content = response.Content;

                    // JSON verisini dynamic kullanarak deserialize ediyoruz
                    dynamic documentData = JsonConvert.DeserializeObject<ExpandoObject>(content, new ExpandoObjectConverter());

                    // Verileri ProductModel'e dönüştürüyoruz
                    var product = new ProductModel
                    {
                        Id = documentId,
                        Name = documentData.fields.name.stringValue,
                        Description = documentData.fields.description.stringValue,
                        Price = documentData.fields.price.stringValue,
                        ImageUrl = documentData.fields.imageUrl.stringValue,

                    };



                    productList.Add(product);
                }
                else
                {
                    Console.WriteLine($"Error retrieving document {documentId}: {response.ErrorMessage}");
                }
            }).ToArray();

            await Task.WhenAll(tasks);  // Tüm sorguların bitmesini bekler

            return productList;
        }

        public async Task<bool> AddDataAsync(object jsonBody, string collectionId)
        {
            var client = new RestClient($"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents");
            var authToken = HttpContext.Current.Request.Cookies["AuthToken"].Value;
            var request = new RestRequest(collectionId, Method.Post);
            request.AddHeader("Authorization", $"Bearer {authToken}");
            request.AddHeader("Content-Type", "application/json");



            request.AddJsonBody(jsonBody); // JSON verisini ekle

            // İsteği gönder
            var response = await client.ExecuteAsync(request);

            return response.IsSuccessful; // Başarı durumunu döndür
        }

        public async Task DeleteDocumentAsync(object queryBody, string collectionId)
        {

            // Sorgu URL'si
            var queryUrl = $"{baseUrl}:runQuery"; // Structured Query endpoint

            // Sorgu parametrelerini JSON formatında hazırlıyoruz

            // POST isteği hazırlıyoruz
            var request = new RestRequest(queryUrl, Method.Post);
            request.AddJsonBody(queryBody);

            // Sorguyu çalıştırıyoruz
            RestResponse queryResponse = await client.ExecuteAsync(request);

            if (queryResponse.IsSuccessful && queryResponse.Content != null)
            {
                var documents = JArray.Parse(queryResponse.Content);

                if (documents != null && documents.Any())
                {
                    foreach (var document in documents)
                    {
                        var documentName = document["document"]["name"].ToString();
                        var documentId = documentName.Split('/').Last();

                        // DELETE isteği hazırlıyoruz
                        var deleteUrl = $"{baseUrl}/{collectionId}/{documentId}";
                        var deleteRequest = new RestRequest(deleteUrl, Method.Delete);
                        RestResponse deleteResponse = await client.ExecuteAsync(deleteRequest);

                        if (deleteResponse.IsSuccessful)
                        {
                            Console.WriteLine($"Belge başarıyla silindi: {documentId}");
                        }
                        else
                        {
                            Console.WriteLine($"Belge silinirken hata oluştu: {deleteResponse.Content}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Eşleşen belge bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine($"Sorgu sırasında hata oluştu: {queryResponse.Content}");
            }
        }

        public async Task<List<JObject>> GetDocumentsAsync(object queryBody, string collectionId)
        {
            // Sorgu URL'si
            var queryUrl = $"{baseUrl}:runQuery"; // Structured Query endpoint

            // POST isteği hazırlıyoruz
            var request = new RestRequest(queryUrl, Method.Post);
            request.AddJsonBody(queryBody);

            // Sorguyu çalıştırıyoruz
            RestResponse queryResponse = await client.ExecuteAsync(request);

            if (queryResponse.IsSuccessful && queryResponse.Content != null)
            {
                var documents = JArray.Parse(queryResponse.Content);
                var documentContents = new List<JObject>(); // Belge içeriği listesini oluşturuyoruz

                if (documents != null && documents.Any())
                {
                    foreach (var document in documents)
                    {
                        // Belge içeriğini alıyoruz
                        var documentContent = document["document"];

                        if (documentContent != null)
                        {
                            // Hem fields hem de name değerlerini içeren yeni bir JObject oluşturuyoruz
                            var combinedContent = new JObject
                            {
                                ["name"] = documentContent["name"],
                                ["fields"] = documentContent["fields"]
                            };

                            documentContents.Add(combinedContent); // İçeriği listeye ekliyoruz
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Eşleşen belge bulunamadı.");
                }

                return documentContents; // Belge içeriklerini döndürüyoruz
            }
            else
            {
                Console.WriteLine($"Sorgu sırasında hata oluştu: {queryResponse.Content}");
                return new List<JObject>(); // Hata durumunda boş bir liste döndür
            }
        }


    }

}









