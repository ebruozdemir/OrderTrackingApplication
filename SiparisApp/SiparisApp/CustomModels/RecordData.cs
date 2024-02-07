using SiparisApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiparisApp.CustomModels
{
    public class RecordData
    {
        public static int? SiparisCount()
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                return db.SIPARIS.Where(x => x.AKTIF == 1).Count();
            }
        }
        public static int? UserCount()
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                return db.KULLANICI.Where(x => x.AKTIF == 1).Count();
            }
        }
        public static int? CustomerCount()
        {
            using (SqlDbEntities db = new SqlDbEntities())
            {
                return db.CARI.Where(x => x.AKTIF == 1).Count();
            }
        }
    }
}