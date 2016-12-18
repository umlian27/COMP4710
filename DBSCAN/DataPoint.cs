using System;

namespace DBSCAN
{
    public class DataPoint
    {
        public ConsumerFlight Flight { get; set; }
        public bool Visited { get; set; }
        public bool InCluster { get; set; }
        public int Cluster { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DataPoint consumerFLight = (DataPoint)obj;

            return Flight.FlightId == consumerFLight.Flight.FlightId;
        }
    }
}
