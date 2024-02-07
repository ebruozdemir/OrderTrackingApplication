using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiparisApp.CustomModels
{
    public class CustomerViewModel
    {
        public class AraModel
        {
            public int? ID { get; set; }

            public int? SAYFA_NO { get; set; }
            public IPagedList<FormModel> SONUCLAR { get; set; }

            public List<SelectListItem> CARI_LIST { get; set; }
        }

        public class FormModel
        {           
            public int? ID { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string CARI_ADI { get; set; }

            [Required(ErrorMessage = "(Zorunlu Alan)")]
            public string ULKE { get; set; }

            public int? EKLEYEN_ID { get; set; }
            public string EKLEYEN { get; set; }
            public DateTime? EKLENME_TARIHI { get; set; }
            public int? GUNCELLEYEN_ID { get; set; }
            public string GUNCELLEYEN { get; set; }            
            public DateTime? GUNCELLENME_TARIHI { get; set; }
            public int? AKTIF { get; set; }
           
        }
    }
}