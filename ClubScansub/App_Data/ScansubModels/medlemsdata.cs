using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.App_Data.ScansubModels
{
    public class medlemsdata
    {
        [Key]
        public int id { get; set; }
        public string fornavn { get; set; }
        public string efternavn { get; set; }
        public string adresse { get; set; }
        public string post { get; set; }
        public string telefonprivat { get; set; }
        public string telefonBil { get; set; }
        public string eMail { get; set; }

        public string Certifikat { get; set; }
        public bool status { get; set; }

        public bool jstatus { get; set; }

        public string cpr { get; set; }
        public string navn { get; set; }
        public string telefon { get; set; }
        public string password { get; set; }
    }

}
