using System;
using System.Collections.Generic;

namespace concertTicketWebCoreMVC.Models
{
    public partial class StubHubCity
    {
        public long Index { get; set; }
        public long? GeoNameId { get; set; }
        public string city { get; set; }
        public string StateCode { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Score { get; set; }
        public string TimeZoneId { get; set; }
        public long? TimeZoneRawOffset { get; set; }
        public string TimeZoneDisplayOffset { get; set; }
    }
}
