using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Trendsepet.Classes;
using Trendsepet.Models;

namespace Trendsepet
{
    public partial class Layout : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsUserLoggedIn())
            {
                authLinks.Visible = false;
                userMenu.Visible = true;
            }
            else
            {
                authLinks.Visible = true;
                userMenu.Visible = false;
            }

           
            
        }
        public void UpdateMenu()
        {

            if (IsUserLoggedIn())
            {
                authLinks.Visible = false;
                userMenu.Visible = true;
            }
            else
            {
                authLinks.Visible = true;
                userMenu.Visible = false;
            }
        }


        

        protected async void LogoutButton_Click(object sender, EventArgs e)
        {
            /*
            var idToken = Session["UserToken"] as string;
            if (!string.IsNullOrEmpty(idToken))
            {
                // Firebase'den çıkış yapma işlemi
                var fireStoreService = new FirestoreService();
                await fireStoreService.SignOutAsync(idToken);
            }

            // Oturumu temizle
            Session["UserToken"] = null;

            // AJAX ile güncellemeyi sağlayan bir JavaScript kodu gönder
            ScriptManager.RegisterStartupScript(this, this.GetType(), "logout", "updateNavbar();", true);
            */
        }

        private bool IsUserLoggedIn()
        {
            var token = Session["UserToken"] as string;
            return !string.IsNullOrEmpty(token);
        }


    }
}
