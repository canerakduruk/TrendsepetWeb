using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trendsepet.Classes;

namespace Trendsepet.Pages
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void btnRegister_Click(object sender, EventArgs e)
        {
            var firestoreService = new FirestoreService();
            try
            {
                // Kullanıcının email ve şifre bilgilerini kullanarak kayıt ol
                string idToken = await firestoreService.SignUpWithEmailPasswordAsync(txtEmail.Text, txtPassword.Text);

                // Kayıt başarılı olursa, token'ı bir session ya da cookie'ye saklayabilirsiniz
                if (!string.IsNullOrEmpty(idToken))
                {
                    Session["UserToken"] = idToken;
                    Response.Redirect("~/Pages/Home.aspx"); // Kayıt başarılı olduğunda yönlendirme
                }
                else
                {
                    throw new Exception("Registration failed. Please check your credentials.");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message; // Hata mesajını kullanıcıya göster
            }
        }
    }
}