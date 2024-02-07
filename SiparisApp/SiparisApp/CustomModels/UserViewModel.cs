using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiparisApp.CustomModels
{
    public class UserViewModel
    {
        public class AraModel
        {
            public string AD { get; set; }
            public string SOYAD { get; set; }



            public int? SAYFA_NO { get; set; }
            public IPagedList<FormModel> SONUCLAR { get; set; }
        }

        public class FormModel
        {
            public FormModel()
            {
                this.BIRIM_LIST = new List<SelectListItem>();
                BIRIM_LIST.Add(new SelectListItem { Text = "Seçiniz", Value = "", Selected = true  });
                BIRIM_LIST.Add(new SelectListItem { Text = "OPEX", Value = "OPEX", Selected = false });
                BIRIM_LIST.Add(new SelectListItem { Text = "SATIŞ", Value = "SATIŞ", Selected = false });
                BIRIM_LIST.Add(new SelectListItem { Text = "EHS", Value = "EHS", Selected = false });

                this.SEVIYE_LIST = new List<SelectListItem>();
                SEVIYE_LIST.Add(new SelectListItem { Text = "Seçiniz", Value = "", Selected = true });
                SEVIYE_LIST.Add(new SelectListItem { Text = "Yetkili", Value = "Admin", Selected = false });
                SEVIYE_LIST.Add(new SelectListItem { Text = "Kullanıcı", Value = "User", Selected = false });             
            }

            public int? ID { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string KULLANICI_ADI { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string SIFRE { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string AD { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string SOYAD { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string BIRIM { get; set; }


            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string SEVIYE { get; set; }

            public int? AKTIF { get; set; }





            public List<SelectListItem> BIRIM_LIST { get; set; }
            public List<SelectListItem> SEVIYE_LIST { get; set; }          
        }

       
    }
}