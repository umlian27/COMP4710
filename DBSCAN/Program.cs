using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DBSCAN
{
    public class Program
    {
        public static IList<DataPoint> allDataPoints;
        public static int currentCluster = 0;

        public static void Main(string[] args)
        {
            double eps = 400;
            int minPts = 10;

            allDataPoints = LoadData("flights.dat");

            for (int i = 0; i < allDataPoints.Count; i++)
            {
                if (!allDataPoints[i].Visited)
                {
                    allDataPoints[i].Visited = true;
                    var neighborPoints = RegionQuery(allDataPoints[i], eps);
                    if (neighborPoints.Count >= minPts)
                    {
                        currentCluster++;
                        ExpandCluster(allDataPoints[i], neighborPoints, eps, minPts);
                    }
                }

                Console.WriteLine("id: " + (i+1) + " visited: " + allDataPoints[i].Visited + " cluster: " + allDataPoints[i].Cluster);
            }

            WriteDataToFile();
            Console.WriteLine("there are " + currentCluster + " clusters");
            Console.WriteLine("completed DBSCAN");

            Console.ReadLine();

        }

        public static IList<DataPoint> LoadData(string fileName)
        {
            var dataPoints = new List<DataPoint>();
            int errorCount = 0;
            using (var flightDataStream = File.OpenText(fileName))
            {
                string line;
                while ((line = flightDataStream.ReadLine()) != null)
                {
                    var dataArray = Regex.Split(line, ",");
                    if (dataArray.Length != 9)
                    {
                        errorCount++;
                    }
                    else
                    {
                        var currentFlight = new ConsumerFlight
                        {
                            FlightId = Int32.Parse(dataArray[0]),
                            SourceAirportId = Int32.Parse(dataArray[1]),
                            SourceLongitude = Double.Parse(dataArray[2]),
                            SourceLatitude = Double.Parse(dataArray[3]),
                            DestinationAirportId = Int32.Parse(dataArray[4]),
                            DestinationLongitude = Double.Parse(dataArray[5]),
                            DestinationLatitude = Double.Parse(dataArray[6]),
                            PointToPointDistance = Double.Parse(dataArray[7]),
                            Price = Double.Parse(dataArray[8])
                        };

                        DataPoint dataPoint = new DataPoint { Flight = currentFlight, Visited = false, InCluster = false };

                        dataPoints.Add(dataPoint);
                    }
                }
            }
            Console.WriteLine("Error Count loading flights: " + errorCount);
            return dataPoints;
        }

        public static void WriteDataToFile()
        {
            var sb = new StringBuilder();
            var fileName = "desnsityData.dat";

            for (int i = 0; i < allDataPoints.Count; i++)
            {

                sb.Append(allDataPoints[i].Flight.FlightId);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.SourceAirportId.ToString());
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.SourceLongitude);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.SourceLatitude);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.DestinationAirportId.ToString());
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.DestinationLongitude);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.DestinationLatitude);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.PointToPointDistance);
                sb.Append(",");
                sb.Append(allDataPoints[i].Flight.Price);
                sb.Append(",");
                sb.Append(allDataPoints[i].Cluster);
                sb.Append("\n");
            }
            File.WriteAllText("densityData.dat", sb.ToString());
            Console.WriteLine("Successfully wrote to file 'densityData.dat'");
        }

        public static void ExpandCluster(DataPoint dataPoint, IList<DataPoint> neighborPoints, double eps, int minPts)
        {
            dataPoint.InCluster = true;
            dataPoint.Cluster = currentCluster;

            int i = 0;

            while (i < neighborPoints.Count)
            {
                if (!neighborPoints[i].Visited)
                {
                    neighborPoints[i].Visited = true;
                    var neighborPoints2 = RegionQuery(neighborPoints[i], eps);
                    if (neighborPoints2.Count >= minPts)
                    {
                        for (int j = 0; j < neighborPoints2.Count; j++)
                        {
                            if (!neighborPoints.Contains(neighborPoints2[j]))
                            {
                                neighborPoints.Add(neighborPoints2[j]);
                            }
                        }
                    }
                }

                if (!neighborPoints[i].InCluster)
                {
                    neighborPoints[i].InCluster = true;
                    neighborPoints[i].Cluster = currentCluster;
                }

                i++;
            }

            Console.WriteLine("there are " + neighborPoints.Count + " neighbors in this cluster");
        }

        public static IList<DataPoint> RegionQuery (DataPoint dataPoint, double eps)
        {
            var pointsInEpsNeighborhood = new List<DataPoint>();

            for (int i = 0; i < allDataPoints.Count; i++)
            {
                var distance = CalculateDistance(dataPoint, allDataPoints[i]);
                if (distance < eps)
                {
                    pointsInEpsNeighborhood.Add(allDataPoints[i]);
                }
            }

            return pointsInEpsNeighborhood;
        }

        public static double CalculateDistance(DataPoint dataPoint1, DataPoint dataPoint2)
        {
            ConsumerFlight flight1 = dataPoint1.Flight;
            ConsumerFlight flight2 = dataPoint2.Flight;
            return Math.Sqrt(Math.Pow(Math.Abs(flight1.Price - flight2.Price), 2) + Math.Pow(Math.Abs(flight1.PointToPointDistance - flight2.PointToPointDistance), 2));
        }

    }
}
