using OfficeOpenXml;
using PagedList;
using SiparisApp.CustomModels;
using SiparisApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace SiparisApp.Controllers
{
    public class HomeController : Controller
    {
        #region Siparisler

        [Authorize(Roles = "Admin,User")]
        public ActionResult Index(SiparisViewModel.AraModel model)
        {
            model.CARI_LIST = GetFromCustomerList();
            int SAYFA_NO = model.SAYFA_NO ?? 1;

            model.SONUCLAR = GetRecordList()
            .Where(x =>
                (string.IsNullOrEmpty(model.PROJE_NO) || x.PROJE_NO == model.PROJE_NO) &&
                (string.IsNullOrEmpty(model.SERI_NO.ToString()) || x.SERI_NO == model.SERI_NO) &&
                (string.IsNullOrEmpty(model.GUC.ToString()) || x.GUC == model.GUC) &&
                (string.IsNullOrEmpty(model.CARI_ID.ToString()) || x.CARI_ID == model.CARI_ID)
            )
           .OrderByDescending(x => x.ID)
           .ToPagedList<SiparisViewModel.FormModel>(SAYFA_NO, 50);

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Home/_List.cshtml", model);
            else
                return View(model);
        }

        private List<SiparisViewModel.FormModel> GetRecordList()
        {
            var model = new List<SiparisViewModel.FormModel>();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                var query = (from t in db.SIPARIS

                             join c in db.CARI on t.CARI_ID equals c.ID into temp1
                             from t1 in temp1.DefaultIfEmpty()

                             join k in db.KULLANICI on t.KULLANICI_ID equals k.ID into temp2
                             from t2 in temp2.DefaultIfEmpty()

                             where
                             t.AKTIF == 1
                             select new
                             {
                                 t.ID,
                                 t.PROJE_NO,
                                 t.SERI_NO,
                                 t.GUC,
                                 CARI_ID = t.CARI_ID,
                                 CARI = t1.CARI_ADI,
                                 t.SIPARIS_ADETI,
                                 t.SIPARIS_TARIHI,
                                 KULLANICI_ID = t.KULLANICI_ID,
                                 KULLANICI = t2.AD + " " + t2.SOYAD,
                                 t.AKTIF
                             }).ToList();

                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        model.Add(new SiparisViewModel.FormModel
                        {
                            ID = item.ID,
                            PROJE_NO = item.PROJE_NO,
                            SERI_NO = item.SERI_NO,
                            GUC = item.GUC,

                            CARI_ID = item.CARI_ID,
                            CARI = item.CARI,

                            SIPARIS_ADETI = item.SIPARIS_ADETI,
                            SIPARIS_TARIHI = item.SIPARIS_TARIHI,

                            KULLANICI_ID = item.KULLANICI_ID,
                            KULLANICI = item.KULLANICI,

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
            SiparisViewModel.FormModel model = null;
            if (id == 0)
            {
                model = new SiparisViewModel.FormModel();
                model.CARI_LIST = GetFromCustomerList();
                model.KULLANICI_LIST = GetFromUserList();
                model.ID = 0;
            }
            else
            {
                using (SqlDbEntities db = new SqlDbEntities())
                {
                    var table = db.SIPARIS.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
                    if (table != null)
                    {
                        model = new SiparisViewModel.FormModel();
                        model.ID = table.ID;
                        model.PROJE_NO = table.PROJE_NO;
                        model.SERI_NO = table.SERI_NO;
                        model.GUC = table.GUC;

                        model.CARI_LIST = GetFromCustomerList();
                        model.CARI_ID = table.CARI_ID;

                        model.SIPARIS_ADETI = table.SIPARIS_ADETI;
                        model.SIPARIS_TARIHI = table.SIPARIS_TARIHI;

                        model.KULLANICI_LIST = GetFromUserList();
                        model.KULLANICI_ID = table.KULLANICI_ID;

                        model.AKTIF = table.AKTIF;
                    }
                }
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(SiparisViewModel.FormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ID == 0)
                    {
                        using (SqlDbEntities db = new SqlDbEntities())
                        {
                            var kayit = new SIPARIS();
                            kayit.PROJE_NO = model.PROJE_NO;
                            kayit.SERI_NO = model.SERI_NO;
                            kayit.GUC = model.GUC;
                            kayit.CARI_ID = model.CARI_ID;
                            kayit.SIPARIS_ADETI = model.SIPARIS_ADETI;
                            kayit.SIPARIS_TARIHI = model.SIPARIS_TARIHI;
                            kayit.KULLANICI_ID = model.KULLANICI_ID;
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
                            var kayit = db.SIPARIS.FirstOrDefault(x => x.ID == model.ID);
                            if (kayit != null)
                            {
                                kayit.PROJE_NO = model.PROJE_NO;
                                kayit.SERI_NO = model.SERI_NO;
                                kayit.GUC = model.GUC;
                                kayit.CARI_ID = model.CARI_ID;
                                kayit.SIPARIS_ADETI = model.SIPARIS_ADETI;
                                kayit.SIPARIS_TARIHI = model.SIPARIS_TARIHI;
                                kayit.KULLANICI_ID = model.KULLANICI_ID;
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
                model.CARI_LIST = GetFromCustomerList();
                model.KULLANICI_LIST = GetFromUserList();

                TempData["Hata"] = "Hala doldurulmamış alanlar var.";
                return View(model);
            }




            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                var table = db.SIPARIS.FirstOrDefault(x => x.AKTIF == 1 && x.ID == id);
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

        private List<SelectListItem> GetFromUserList()
        {
            var list = new List<SelectListItem>();

            using (SqlDbEntities db = new SqlDbEntities())
            {
                list = (from t in db.KULLANICI
                        where
                        t.AKTIF == 1
                        orderby t.AD
                        select new SelectListItem
                        {
                            Text = t.AD + " " + t.SOYAD,
                            Value = t.ID.ToString(),
                        }).ToList();

                list.Insert(0, new SelectListItem { Text = "Seçiniz", Value = "", Selected = true });
            }
            return list;
        }

        public ActionResult ExceleAktar(string PROJE_NO, int? SERI_NO, int? GUC, int? CARI_ID)
        {
            bool success = false;
            string message = "Kayıt oluşturulmadı.";

            try
            {
                var liste = GetRecordList()
                    .Where(x =>
                        (string.IsNullOrEmpty(PROJE_NO) || x.PROJE_NO == PROJE_NO) &&
                        (string.IsNullOrEmpty(SERI_NO.ToString()) || x.SERI_NO == SERI_NO) &&
                        (string.IsNullOrEmpty(GUC.ToString()) || x.GUC == GUC) &&
                        (string.IsNullOrEmpty(CARI_ID.ToString()) || x.CARI_ID == CARI_ID)
                      )
                    .OrderByDescending(o => o.ID)
                    .ToList();

                if (liste.Any())
                {
                    success = true;
                    message = "Kayıt oluşturuldu.";

                    //ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var excelPackage = new ExcelPackage())
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("Sayfa1");
                        sheet.Cells["A1"].Value = "ID";
                        sheet.Cells["B1"].Value = "PROJE NO";
                        sheet.Cells["C1"].Value = "SERİ NO";
                        sheet.Cells["D1"].Value = "GÜÇ";
                        sheet.Cells["E1"].Value = "CARİ ADI";
                        sheet.Cells["F1"].Value = "SİPARİŞ ADETİ";
                        sheet.Cells["G1"].Value = "SİPARİŞ TARİHİ";
                        sheet.Cells["H1"].Value = "SATIŞÇISI";

                        int row = 2;

                        foreach (var item in liste)
                        {
                            sheet.Cells[$"A{row}"].Value = item.ID;
                            sheet.Cells[$"B{row}"].Value = item.PROJE_NO;
                            sheet.Cells[$"C{row}"].Value = item.SERI_NO;
                            sheet.Cells[$"D{row}"].Value = item.GUC;
                            sheet.Cells[$"E{row}"].Value = item.CARI;
                            sheet.Cells[$"F{row}"].Value = item.SIPARIS_ADETI;
                            sheet.Cells[$"G{row}"].Value = item.SIPARIS_TARIHI.Value.ToShortDateString();
                            sheet.Cells[$"H{row}"].Value = item.KULLANICI;

                            row++;
                        }

                        sheet.Cells["A1:H1"].Style.Font.Size = 10;
                        sheet.Cells["A1:H1"].Style.Font.Bold = true;
                        sheet.View.FreezePanes(2, 1);
                        sheet.Cells["A1:H1"].AutoFilter= true;
                        sheet.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                        sheet.Cells["A:AZ"].AutoFitColumns();
                        sheet.Protection.IsProtected = false;
                        sheet.Protection.AllowSelectLockedCells = false;

                        for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                        {
                            if (sheet.Column(i).Width > 40)
                                sheet.Column(i).Width = 40;
                        }



                        Session["Download"] = excelPackage.GetAsByteArray();
                        Session["FileName"] = "Excel_Liste.xlsx";
                    }                   
                }               
            }
            catch (Exception)
            {
            }
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download()
        {
            if (Session["Download"] != null && Session["FileName"] != null)
            {
                string FileName = Session["FileName"].ToString();
                byte[] data = Session["Download"] as byte[];

                Session.Remove("Download");
                Session.Remove("FileName");
                Session.Remove("FileType");

                return File(data, "application/octet-stream", FileName);
            }
            else
                return new EmptyResult();
        }

        #endregion
    }
}