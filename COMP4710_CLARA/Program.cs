using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace COMP4710_CLARA
{
    public class Program
    {
        public static double sampleAvgMinDistance = Double.MaxValue;
        public static StringBuilder chosenSampleSb;
        public static int sampleNum = 0;
        public static int chosenSampleNum = 0;

        static void Main(string[] args)
        {
            IList<ConsumerFlight> allConsumerFlights = LoadFlights("flights.dat");
            int numMedoids = 5;
            int sampleSize = 100;
            int numSamples = 5;

            for (int i = 0; i < numSamples; i++)
            {
                var randomSampleFlights = GetRandomFlights(allConsumerFlights, sampleSize);
                var randomSampleMedoids = ChooseMedoids(randomSampleFlights, numMedoids);
                var medoidsAfterPAM = RunPAM(randomSampleFlights, randomSampleMedoids);
                WriteFlightDataWithMedoids(allConsumerFlights, medoidsAfterPAM);
            }

            Console.WriteLine("the chosen sample num is " + chosenSampleNum);


            File.WriteAllText("bestSample.dat", chosenSampleSb.ToString());
            Console.WriteLine("Wrote data to the file 'bestSample.dat'");

            Console.ReadLine();
        }

        public static IList<ConsumerFlight> LoadFlights(string fileName)
        {
            var consumerFlights = new List<ConsumerFlight>();
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
                        var currentFlight = new ConsumerFlight()
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
                        consumerFlights.Add(currentFlight);
                    }
                }
            }
            Console.WriteLine("Error Count loading flights: " + errorCount);
            return consumerFlights;
        }

        public static IList<ConsumerFlight> ChooseMedoids(IList<ConsumerFlight> consumerFlights, int numMedoids)
        {
            var medoids = new List<ConsumerFlight>();

            if (consumerFlights != null && numMedoids > 0 && numMedoids < consumerFlights.Count)
            {
                Random random = new Random();
                for (int i = 0; i < numMedoids; i++)
                {
                    int randomInt = random.Next(0, consumerFlights.Count);
                    medoids.Add(consumerFlights[randomInt]);
                }
            }

            return medoids;
        }

        public static IList<ConsumerFlight> RunPAM(IList<ConsumerFlight> consumerFlights, IList<ConsumerFlight> medoids)
        {
            var newMedoids = new List<ConsumerFlight>(medoids);
            var changed = true;

            while (changed)
            {
                changed = false;
                for (int i = 0; i < medoids.Count; i++)
                {
                    double minTotalCost = Double.MaxValue;
                    ConsumerFlight lowestCostNonMedoid = null;

                    for (int j = 0; j < consumerFlights.Count; j++)
                    {
                        if (!medoids[i].Equals(consumerFlights[j]))
                        {
                            //Console.WriteLine("comparing medoid " + medoids[i].FlightId + " with non-medoid " + consumerFlights[j].FlightId);
                            var currTotalCost = CalculateTotalSwapCost(medoids[i], consumerFlights[j], consumerFlights, medoids);
                            if (currTotalCost < minTotalCost)
                            {
                                minTotalCost = currTotalCost;
                                lowestCostNonMedoid = consumerFlights[j];
                                //Console.WriteLine("the total cost of switching " + medoids[i].FlightId + " and " + consumerFlights[j].FlightId + " is " + currTotalCost);
                            }
                        }
                    }

                    if (minTotalCost < 0 && lowestCostNonMedoid != null)
                    {
                        //Console.WriteLine("replacing medoid " + medoids[i].FlightId + " with " + lowestCostNonMedoid.FlightId + " and min cost of " + minTotalCost + "\n");
                        medoids[i] = lowestCostNonMedoid;
                        changed = true;
                    }
                }
            }

            return newMedoids;
        }

        public static IList<ConsumerFlight> GetRandomFlights(IList<ConsumerFlight> consumerFlights, int numFlights)
        {
            var randomFlights = new List<ConsumerFlight>();

            if (consumerFlights != null && numFlights > 0)
            {
                Random random = new Random();

                for (int i = 0; i < numFlights; i++)
                {
                    int randomInt = random.Next(0, consumerFlights.Count);
                    randomFlights.Add(consumerFlights[randomInt]);
                }
            }

            return randomFlights;
        } 

        public static void WriteFlightDataWithMedoids(IList<ConsumerFlight> consumerFlights, IList<ConsumerFlight> medoids)
        {
            var sb = new StringBuilder();
            var fileName = "flightsWithMedoids.dat";
            double totalMinDistance = 0;
            double averageMinDistance;

            for (int i = 0; i < consumerFlights.Count; i++)
            {
                ConsumerFlight minDistanceMedoid = null;
                double minDistance = Double.MaxValue; //distance between current flight and its closest medoid

                for (int j = 0; j < medoids.Count; j++)
                {
                    var distance = CalculateDistance(consumerFlights[i], medoids[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistanceMedoid = medoids[j];
                    }
                }

                totalMinDistance += minDistance;

                sb.Append(consumerFlights[i].FlightId);
                sb.Append(",");
                sb.Append(consumerFlights[i].SourceAirportId.ToString());
                sb.Append(",");
                sb.Append(consumerFlights[i].SourceLongitude);
                sb.Append(",");
                sb.Append(consumerFlights[i].SourceLatitude);
                sb.Append(",");
                sb.Append(consumerFlights[i].DestinationAirportId.ToString());
                sb.Append(",");
                sb.Append(consumerFlights[i].DestinationLongitude);
                sb.Append(",");
                sb.Append(consumerFlights[i].DestinationLatitude);
                sb.Append(",");
                sb.Append(consumerFlights[i].PointToPointDistance);
                sb.Append(",");
                sb.Append(consumerFlights[i].Price);
                sb.Append(",");
                sb.Append(minDistanceMedoid.FlightId);
                sb.Append("\n");
            }

            averageMinDistance = totalMinDistance / consumerFlights.Count;


            if (averageMinDistance < sampleAvgMinDistance)
            {
                sampleAvgMinDistance = averageMinDistance;
                chosenSampleSb = sb;
                chosenSampleNum = sampleNum;
            }

            sampleNum++;
            Console.WriteLine("the avg distance for sample " + sampleNum + " is " + averageMinDistance);

        }


        public static double CalculateTotalSwapCost(ConsumerFlight medoid, ConsumerFlight potentialMedoid, IList<ConsumerFlight> consumerFlights, IList<ConsumerFlight> medoids)
        {
            double totalCost = 0;

            foreach (ConsumerFlight flight in consumerFlights)
            {
                if (!medoids.Contains(flight) && !flight.Equals(potentialMedoid))
                {
                    var cost = CalculateNonMedoidSwapCost(flight, medoid, potentialMedoid, consumerFlights, medoids);
                    totalCost += cost;
                }
            }
            return totalCost;
        }

        public static double CalculateNonMedoidSwapCost(ConsumerFlight targetNonMedoid, ConsumerFlight medoid, ConsumerFlight potentialMedoid, IList<ConsumerFlight> consumerFlights, IList<ConsumerFlight> medoids)
        {
            double closestDistance = Double.MaxValue;
            double secondClosestDistance = Double.MaxValue;
            double switchedDistance = CalculateDistance(targetNonMedoid, potentialMedoid);
            double cost;
            ConsumerFlight closestMedoid = null;


            //finding the medoid that targetNonMedoid belongs to
            foreach(var m in medoids)
            {
                var distance = CalculateDistance(targetNonMedoid, m);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMedoid = m;
                }
            }

            if (closestMedoid.Equals(medoid))
            {
                //find second closest distance
                foreach (var m in medoids)
                {
                    if (!m.Equals(medoid))
                    {
                        var distance = CalculateDistance(targetNonMedoid, m);
                        if (distance < secondClosestDistance)
                        {
                            secondClosestDistance = distance;
                        }
                    }
                }

                cost = switchedDistance < secondClosestDistance
                    ? switchedDistance - closestDistance
                    : secondClosestDistance - closestDistance;

            }
            else
            {
                cost = switchedDistance < closestDistance ? switchedDistance - closestDistance : 0;

            }

            return cost;
        }

        public static double CalculateDistance(ConsumerFlight flight1, ConsumerFlight flight2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(flight1.Price - flight2.Price), 2) + Math.Pow(Math.Abs(flight1.PointToPointDistance - flight2.PointToPointDistance), 2));
        }

    }

}
