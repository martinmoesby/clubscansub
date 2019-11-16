using System;
using System.Collections.Generic;
using System.Text;

namespace ClubScansub.Models.Structs
{
    public struct MapCoordinates
    {
        public string latitude { get; set; }
        public string longitude { get; set; }

    }

    public struct RoutePoints
    {
        public string StartAddress {get; set;}
        public string EndAddress { get; set; }

        public MapCoordinates Start { get; set; }
        public MapCoordinates End { get; set; }
    }

}
