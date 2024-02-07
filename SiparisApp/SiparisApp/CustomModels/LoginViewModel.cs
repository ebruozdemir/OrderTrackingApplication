using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiparisApp.CustomModels
{
    public class LoginViewModel
    {
        [Required]
        public string KULLANICI_ADI { get; set; }

        [Required]
        public string SIFRE { get; set; }
    }
}