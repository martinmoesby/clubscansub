using System;
using System.Collections.Generic;
using System.Text;

namespace ClubScansub.Models.Interface
{
    internal interface IAdress
    {
        string City { get; set; }
        string Country { get; set; }
        string PostalCode { get; set; }
        string State { get; set; }
        string Street { get; set; }
    }
}
