using PagedList;
using SiparisApp.CustomModels;
using SiparisApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SiparisApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        #region Kullanıcı

        public ActionResult Index(UserViewModel.AraModel model)
        {
            int SAYFA_NO = model.SAYFA_NO ?? 1;

            model.SONUCLAR = GetRecordList()
            .Where(x =>
                (string.IsNullOrEmpty(model.AD) || x.AD.Contains(model.AD)) &&
                (string.IsNullOrEmpty(model.SOYAD) || x.SOYAD.Contains(model.SOYAD))
            )
           .OrderByDescending(x => x.ID)
           .ToPagedList<UserViewModel.FormModel>(SAYFA_NO, 50);

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/User/_List.cshtml", model);
            else
                return View(model);
        }

        private List<UserViewModel.FormModel> GetRecordList()
        {
            var model = new List<UserViewModel.FormModel>();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                var query = (from t in db.KULLANICI

                             where
                             t.AKTIF == 1
                             select new
                             {
                                 t.ID,
                                 t.KULLANICI_ADI,
                                 t.SIFRE,
                                 t.AD,
                                 t.SOYAD,
                                 t.BIRIM,
                                 t.SEVIYE,
                                 t.AKTIF,

                             }).ToList();

                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        model.Add(new UserViewModel.FormModel
                        {
                            ID = item.ID,
                            KULLANICI_ADI = item.KULLANICI_ADI,
                            SIFRE = item.SIFRE,
                            AD = item.AD,
                            SOYAD = item.SOYAD,
                            BIRIM = item.BIRIM,
                            SEVIYE = item.SEVIYE,
                            AKTIF = item.AKTIF,


                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public ActionResult Save(int id = 0)
        {
            UserViewModel.FormModel model = null;
            if (id == 0)
            {
                model = new UserViewModel.FormModel();
                model.ID = 0;
            }
            else
            {
                using (SqlDbEntities db = new SqlDbEntities())
                {
                    var table = db.KULLANICI.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
                    if (table != null)
                    {
                        model = new UserViewModel.FormModel();
                        model.ID = table.ID;
                        model.KULLANICI_ADI = table.KULLANICI_ADI;
                        model.SIFRE = table.SIFRE;
                        model.AD = table.AD;
                        model.SOYAD = table.SOYAD;
                        model.BIRIM = table.BIRIM;
                        model.SEVIYE = table.SEVIYE;
                        model.AKTIF = table.AKTIF;
                    }
                }
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(UserViewModel.FormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ID == 0)
                    {
                        using (SqlDbEntities db = new SqlDbEntities())
                        {
                            var kayit = new KULLANICI();
                            kayit.KULLANICI_ADI = model.KULLANICI_ADI;
                            kayit.SIFRE = model.SIFRE;
                            kayit.AD = model.AD;
                            kayit.SOYAD = model.SOYAD;
                            kayit.BIRIM = model.BIRIM;
                            kayit.SEVIYE = model.SEVIYE;
                            kayit.AKTIF = 1;



                            db.Entry(kayit).State = EntityState.Added;
                            db.SaveChanges();
                            TempData["Mesaj"] = "Kayıt eklendi.";
                        }
                    }
                    else
                    {
                        using (SqlDbEntities db = new SqlDbEntities())
                        {
                            var kayit = db.KULLANICI.FirstOrDefault(x => x.ID == model.ID);
                            if (kayit != null)
                            {
                                kayit.KULLANICI_ADI = model.KULLANICI_ADI;
                                kayit.SIFRE = model.SIFRE;
                                kayit.AD = model.AD;
                                kayit.SOYAD = model.SOYAD;
                                kayit.BIRIM = model.BIRIM;
                                kayit.SEVIYE = model.SEVIYE;
                                kayit.AKTIF = model.AKTIF;



                                db.Entry(kayit).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Mesaj"] = "Kayıt güncellendi.";
                            }
                        }
                    }
                }
                finally
                {
                }
            }
            else
            {
                TempData["Hata"] = "Hala doldurulmamış alanlar var.";
                return View(model);
            }
            return RedirectToAction("Index", "User");
        }


        public ActionResult Delete(int id)
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                var table = db.KULLANICI.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
                if (table != null)
                {
                    table.AKTIF = 0;

                    db.Entry(table).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "Kayıt silindi." }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Profil

        public ActionResult UserProfile()
        {
            var model = new UserViewModel.FormModel();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                var query = (from t in db.KULLANICI

                             where
                             t.AKTIF == 1 &&
                             t.KULLANICI_ADI == HttpContext.User.Identity.Name
                             select new
                             {
                                 t.ID,
                                 t.KULLANICI_ADI,
                                 t.SIFRE,
                                 t.AD,
                                 t.SOYAD,
                                 t.BIRIM,
                                 t.SEVIYE,
                                 t.AKTIF,

                             }).FirstOrDefault();

                if (query != null)
                {
                    model.ID = query.ID;
                    model.KULLANICI_ADI = query.KULLANICI_ADI;
                    model.SIFRE = query.SIFRE;
                    model.AD = query.AD;
                    model.SOYAD = query.SOYAD;
                    model.BIRIM = query.BIRIM;
                    model.SEVIYE = query.SEVIYE;
                    model.AKTIF = query.AKTIF;
                }
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UserProfile(UserViewModel.FormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlDbEntities db = new SqlDbEntities())
                    {
                        var kayit = db.KULLANICI.FirstOrDefault(x => x.ID == model.ID);
                        if (kayit != null)
                        {
                            kayit.KULLANICI_ADI = model.KULLANICI_ADI;
                            kayit.SIFRE = model.SIFRE;
                            kayit.AD = model.AD;
                            kayit.SOYAD = model.SOYAD;
                            kayit.BIRIM = model.BIRIM;
                            kayit.SEVIYE = model.SEVIYE;
                            kayit.AKTIF = model.AKTIF;


                            db.Entry(kayit).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["Mesaj"] = "Kayıt güncellendi.";
                        }
                    }
                }
                finally
                {
                }
            }
            else
            {
                TempData["Hata"] = "Hala doldurulmamış alanlar var.";
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}