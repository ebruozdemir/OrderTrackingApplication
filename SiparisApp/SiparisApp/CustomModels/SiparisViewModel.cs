using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiparisApp.CustomModels
{
    public class SiparisViewModel
    {
        public class AraModel
        {
            public string PROJE_NO { get; set; }
            public int? SERI_NO { get; set; }
            public int? GUC { get; set; }
            public int? CARI_ID { get; set; }


            public int? SAYFA_NO { get; set; }
            public IPagedList<FormModel> SONUCLAR { get; set; }
            public List<SelectListItem> CARI_LIST { get; set; }
        }

        public class FormModel
        {
            public int? ID { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string PROJE_NO { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public int? SERI_NO { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public int? GUC { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public int? CARI_ID { get; set; }
            public string CARI { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public int? SIPARIS_ADETI { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public DateTime? SIPARIS_TARIHI { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public int? KULLANICI_ID { get; set; }
            public string KULLANICI { get; set; }

            public int? AKTIF { get; set; }

           


            public List<SelectListItem> CARI_LIST { get; set; }
            public List<SelectListItem> KULLANICI_LIST { get; set; }
        }
    }
}