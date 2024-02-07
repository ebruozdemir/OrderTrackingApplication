using SiparisApp.CustomModels;
using SiparisApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SiparisApp.Controllers
{
    [AllowAnonymous]
    public class SecurityController : Controller
    {
        #region Kullanici

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                var userControl = db.KULLANICI.FirstOrDefault(x => x.AKTIF == 1 && x.KULLANICI_ADI == model.KULLANICI_ADI && x.SIFRE == model.SIFRE);
                if (userControl != null)
                {
                    FormsAuthentication.SetAuthCookie(model.KULLANICI_ADI, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Kullanıcı adı ya da şifre hatalı.";
                    return View(model);
                }
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Security");
        }

        #endregion

    }
}