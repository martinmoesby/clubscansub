using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.App_Data.ScansubModels
{
    public class saldooplysning
    {
        public int id { get; set; }
        public string password { get; set; }
        public DateTime dato { get; set; }
        public string tekst { get; set; }
        public int turid { get; set; }
        public decimal pris { get; set; }
        public string dsfnr { get; set; }
    }
}
