using PagedList;
using SiparisApp.CustomModels;
using SiparisApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiparisApp.Controllers
{
    public class CustomerController : Controller
    {
        #region Musteri

        [Authorize(Roles = "Admin")]
        public ActionResult Index(CustomerViewModel.AraModel model)
        {
            model.CARI_LIST = GetFromCustomerList();

            int SAYFA_NO = model.SAYFA_NO ?? 1;

            model.SONUCLAR = GetRecordList()
            .Where(x =>
                 (string.IsNullOrEmpty(model.ID.ToString()) || x.ID == model.ID)
            )
           .OrderByDescending(x => x.ID)
           .ToPagedList<CustomerViewModel.FormModel>(SAYFA_NO, 50);

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Customer/_List.cshtml", model);
            else
                return View(model);
        }


        private List<CustomerViewModel.FormModel> GetRecordList()
        {
            var model = new List<CustomerViewModel.FormModel>();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                var query = (from t in db.CARI
                             join k1 in db.KULLANICI on t.EKLEYEN_ID equals k1.ID into temp1
                             from t1 in temp1.DefaultIfEmpty()
                             join k2 in db.KULLANICI on t.GUNCELLEYEN_ID equals k2.ID into temp2
                             from t2 in temp2.DefaultIfEmpty()
                             where
                             t.AKTIF == 1
                             select new
                             {
                                 t.ID,
                                 t.CARI_ADI,
                                 t.ULKE,
                                 t.EKLEYEN_ID,
                                 EKLEYEN = t1.AD + " " + t1.SOYAD,
                                 t.EKLENME_TARIHI,
                                 t.GUNCELLEYEN_ID,
                                 GUNCELLEYEN = t2.AD + " " + t2.SOYAD,
                                 t.GUNCELLENME_TARIHI,
                                 t.AKTIF
                             }).ToList();

                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        model.Add(new CustomerViewModel.FormModel
                        {
                            ID = item.ID,
                            CARI_ADI = item.CARI_ADI,
                            ULKE = item.ULKE,
                            EKLEYEN = item.EKLEYEN,
                            EKLEYEN_ID = item.EKLEYEN_ID,
                            EKLENME_TARIHI = item.EKLENME_TARIHI,
                            GUNCELLEYEN = item.GUNCELLEYEN,
                            GUNCELLEYEN_ID = item.GUNCELLEYEN_ID,
                            GUNCELLENME_TARIHI = item.GUNCELLENME_TARIHI,
                            AKTIF = item.AKTIF
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public ActionResult Save(int id = 0)
        {
            CustomerViewModel.FormModel model = null;
            if (id == 0)
            {
                model = new CustomerViewModel.FormModel();
                model.ID = 0;
            }
            else
            {
                using (SqlDbEntities db = new SqlDbEntities())
                {
                    var table = db.CARI.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
                    if (table != null)
                    {
                        model = new CustomerViewModel.FormModel();
                        model.ID = table.ID;
                        model.CARI_ADI = table.CARI_ADI;
                        model.ULKE = table.ULKE;
                        model.EKLEYEN_ID = table.EKLEYEN_ID;
                        model.EKLENME_TARIHI = table.EKLENME_TARIHI;
                        model.GUNCELLEYEN_ID = table.GUNCELLEYEN_ID;
                        model.GUNCELLENME_TARIHI = table.GUNCELLENME_TARIHI;
                        model.AKTIF = table.AKTIF;
                    }
                }
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(CustomerViewModel.FormModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.CARI_ADI))
                {
                    TempData["Hata"] = "Müşteri adı boş geçilemez.";

                    return View(model);
                }

                if (CustomerControl(model) == false)
                {
                    TempData["Hata"] = "Bu müşteri zaten kayıtlı";

                    return View(model);
                }

                var user = GetUserDetails(HttpContext.User.Identity.Name);

                try
                {
                    if (model.ID == 0)
                    {
                        using (SqlDbEntities db = new SqlDbEntities())
                        {
                            var kayit = new CARI();
                            kayit.ID = (int)model.ID;
                            kayit.CARI_ADI = model.CARI_ADI;
                            kayit.ULKE = model.ULKE;
                            kayit.EKLEYEN_ID = user.ID;
                            kayit.EKLENME_TARIHI = DateTime.Now;
                            kayit.GUNCELLEYEN_ID = null;
                            kayit.GUNCELLENME_TARIHI = null;
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
                            var kayit = db.CARI.FirstOrDefault(x => x.ID == model.ID);
                            if (kayit != null)
                            {
                                kayit.ID = (int)model.ID;
                                kayit.CARI_ADI = model.CARI_ADI;
                                kayit.ULKE = model.ULKE;
                                kayit.EKLEYEN_ID = model.EKLEYEN_ID;
                                kayit.EKLENME_TARIHI = model.EKLENME_TARIHI;
                                kayit.GUNCELLEYEN_ID = user.ID;
                                kayit.GUNCELLENME_TARIHI = DateTime.Now;
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
            return RedirectToAction("Index", "Customer");
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                var table = db.CARI.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
                if (table != null)
                {
                    table.AKTIF = 0;

                    db.Entry(table).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "Kayıt silindi." }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetFromCustomerList()
        {
            var list = new List<SelectListItem>();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                list = (from t in db.CARI
                        where
                        t.AKTIF == 1
                        orderby t.CARI_ADI
                        select new SelectListItem
                        {
                            Text = t.CARI_ADI,
                            Value = t.ID.ToString(),
                        }).ToList();

                list.Insert(0, new SelectListItem { Text = "Seçiniz", Value = "", Selected = true });
            }
            return list;
        }

        private bool CustomerControl(CustomerViewModel.FormModel model)
        {
            bool state = false;

            try
            {
                using (SqlDbEntities db = new SqlDbEntities())
                {
                    var recordList = db.CARI
                            .Where(x => x.AKTIF == 1 && x.CARI_ADI == model.CARI_ADI)
                            .ToList();

                    if (recordList.Any())
                    {
                        if (model.ID == 0)
                        {
                            if (recordList.Count > 0)
                                state = false;
                            else
                                state = true;
                        }
                        else
                        {
                            if (recordList.Count == 0)
                                state = true;
                            else if (recordList.Count == 1)
                            {
                                var control = recordList.FirstOrDefault(x => x.ID == model.ID);
                                if (control != null)
                                    state = true;
                                else
                                    state = false;
                            }
                            else if (recordList.Count > 1)
                                state = false;
                        }
                    }
                    else
                        state = true;
                }
            }
            catch (Exception) { }

            return state;
        }

        private KULLANICI GetUserDetails(string UserName)
        {
            KULLANICI details = null;

            using (SqlDbEntities db = new SqlDbEntities())
            {
                var user = db.KULLANICI.FirstOrDefault(x => x.AKTIF == 1 && x.KULLANICI_ADI == UserName);
                if (user != null)
                    details = user;
            }

            return details;
        }

        #endregion
    }
}