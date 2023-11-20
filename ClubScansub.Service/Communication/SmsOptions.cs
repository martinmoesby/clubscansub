using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Service.Communication
{
    public class SmsOptions { 
        public string AuthenticationToken { get; set; }
        public string Host { get; set; }
        public string From { get; set; }
    }
}
