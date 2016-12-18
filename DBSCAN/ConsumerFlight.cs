using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSCAN
{
    public class ConsumerFlight
    {
        public int FlightId { get; set; }
        public int SourceAirportId { get; set; }
        public double SourceLongitude { get; set; }
        public double SourceLatitude { get; set; }
        public int DestinationAirportId { get; set; }
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double PointToPointDistance { get; set; }
        public double Price { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ConsumerFlight consumerFLight = (ConsumerFlight) obj;
            return FlightId == consumerFLight.FlightId;
        }
    }

}
