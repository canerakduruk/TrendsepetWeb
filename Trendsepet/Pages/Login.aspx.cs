using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trendsepet.Classes;

namespace Trendsepet.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            var firestoreService = new FirestoreService();

            try
            {
                // Kullanıcının email ve şifre bilgilerini kullanarak oturum aç
                string idToken = await firestoreService.SignInWithEmailPasswordAsync(txtEmail.Text, txtPassword.Text);

                // Eğer oturum açma başarılı olursa
                if (!string.IsNullOrEmpty(idToken))
                {
                    // Token'ı bir session ya da cookie'ye saklayabilirsiniz
                    Session["UserToken"] = idToken;

                    // Giriş başarılı olduğunda yönlendirme yapabilirsiniz
                    // MasterPage'e erişim
                    var master = Master as Layout;
                    if (master != null)
                    {
                        master.UpdateMenu();
                    }
                }
                else
                {
                    // Eğer token boşsa, oturum açma başarısızdır
                    throw new Exception("Login failed. Please check your credentials.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // HTTP isteğiyle ilgili hata (ağ sorunları, sunucuya ulaşılamaması)
                txtEmail.Text = "HTTP Error: " + httpEx.Message;
            }
            catch (JsonException jsonEx)
            {
                // JSON işleme hatası (API yanıtı beklendiği gibi değilse)
                txtEmail.Text = "JSON Error: " + jsonEx.Message;
            }
            catch (Exception ex)
            {
                // Yanıtın detaylarını loglayın
                txtEmail.Text = $"General Error: {ex.Message}. Response Details: {ex.InnerException?.Message}";
            }
        }
    }
}