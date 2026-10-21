using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.Wasm;
using System.Text;

namespace Garage
{
    internal class ParkingComplex
    {
        private Garage[] garages;

        internal struct SearchTerms
        {
            public string Type = string.Empty;
            public string Licence = string.Empty;
            public Color Color = Color.Empty;
            public int NumberOfWheels = int.MinValue;
            public Car.Drive DriveType = Car.Drive.Blank;
            public int SeatHeight = int.MinValue;
            public int NumberOfSeats = int.MinValue;
            public int Wingspan = int.MinValue;
            public int Length = int.MinValue;

            public SearchTerms()
            {
            }
        }

        internal ParkingComplex()
        {
            garages = new Garage[0];
        }

        internal void CreateGarage(int capacity = 10)
        {
            //resize garages to make room for new garage
            Array.Resize(ref garages, garages.Length + 1);

            //add the new garages at the end
            garages[garages.Length - 1] = new(capacity);
        }

        internal void RemoveGarage(int index)
        {
            if (index >= garages.Length)
            {
                throw new IndexOutOfRangeException($"Garage {index} does not exist.");
            }

            garages[index].EmptyGarage();
            garages[index] = null;

            PackAndTrimGarages();
        }

        private void PackAndTrimGarages()
        {
            //find the gap in the array
            int index = Array.IndexOf(garages, null);

            //move the last garage into the gap, if the last spot was the empty sput this just assigns to itself
            garages[index] = garages[^1];

            //trim the last slot in the array, since it's moved
            Array.Resize(ref garages, garages.Length - 1);
        }

        internal bool PopulateGarage(int index, int numberOfVehicles)
        {
            if (index >= garages.Length)
            {
                throw new IndexOutOfRangeException($"Garage {index} does not exist.");
            }

            return garages[index].PopulateGarage(numberOfVehicles);
        }

        internal Garage GetGarage(int number)
        {
            if (number >= garages.Length)
            {
                throw new IndexOutOfRangeException("Garage does not exist.");
            }
            else
            {
                return garages[number];
            }
        }

        internal Garage[] GetAllGarages()
        {
            return garages;
        }

        internal bool RemoveVehicle(string licence)
        {
            bool success = false;

            foreach (Garage garage in garages)
            {
                success = success || garage.RemoveVehicle(licence);
            }

            return success;
        }

        internal bool ParkVehicle(Vehicle vehicle, int garage)
        {
            return garages[garage].ParkVehicle(vehicle);
        }

        internal Vehicle[] SearchGarages(SearchTerms searchTerms)
        {
            Vehicle[] result = new Vehicle[50];
            int resultIndex = 0;

            foreach (Garage garage in garages)
            {
                foreach (Vehicle vehicle in garage.ParkingSpaces)
                {
                    if (vehicle is not null)
                    {
                        if (IsMatch(vehicle, searchTerms))
                        {
                            if (result.Length == resultIndex)
                            {
                                //result array is full
                                Array.Resize(ref result, result.Length * 2);
                            }

                            result[resultIndex] = vehicle;
                            resultIndex++;
                        }
                    }
                }
            }

            //trim result array
            //resultIndex should be the index position behind the last element, hence the filled length of the array
            Array.Resize(ref result, resultIndex);

            return result;
        }

        private bool IsMatch(Vehicle vehicle, SearchTerms searchTerms)
        {
            //All terms are considered required so if one doesn't match there is no match.

            if (!string.IsNullOrWhiteSpace(searchTerms.Licence))
            {
                if (!string.Equals(vehicle.RegistrationNumber, searchTerms.Licence, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerms.Type))
            {
                if (string.Equals("car", searchTerms.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (vehicle is not Car)
                    {
                        return false;
                    }
                }
                else if (string.Equals("motorcycle", searchTerms.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (vehicle is not Motorcycle)
                    {
                        return false;
                    }
                }
                else if (string.Equals("bus", searchTerms.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (vehicle is not Bus)
                    {
                        return false;
                    }
                }
                else if (string.Equals("airplane", searchTerms.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (vehicle is not Airplane)
                    {
                        return false;
                    }
                }
                else if (string.Equals("boat", searchTerms.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (vehicle is not Boat)
                    {
                        return false;
                    }
                }
                else
                {
                    //The search term is unknown and thus nothing matches
                    return false;
                }
            }

            if (searchTerms.Color != Color.Empty)
            {
                if (!string.Equals(vehicle.Color.Name, searchTerms.Color.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (searchTerms.NumberOfWheels != int.MinValue)
            {
                if (vehicle.NumberOfWheels != searchTerms.NumberOfWheels)
                {
                    return false;
                }
            }

            if (searchTerms.DriveType != Car.Drive.Blank)
            {
                //only cars have drive type all others fail
                if (vehicle is not Car)
                {
                    return false;
                }

                if ((vehicle as Car).TypeOfDrive != searchTerms.DriveType)
                {
                    return false;
                }
            }

            if (searchTerms.SeatHeight != int.MinValue)
            {
                //only motorcycles have seat height all others fail
                if (vehicle is not Motorcycle)
                {
                    return false;
                }

                if ((vehicle as Motorcycle).SeatHeight != searchTerms.SeatHeight)
                {
                    return false;
                }
            }

            if (searchTerms.NumberOfSeats != int.MinValue)
            {
                //only buses have number of seats all others fail
                if (vehicle is not Bus)
                {
                    return false;
                }

                if ((vehicle as Bus).NumberOfSeats != searchTerms.NumberOfSeats)
                {
                    return false;
                }
            }

            if (searchTerms.Wingspan != int.MinValue)
            {
                //only airplanes have wingspan all others fail
                if (vehicle is not Airplane)
                {
                    return false;
                }

                if ((vehicle as Airplane).Wingspan != searchTerms.Wingspan)
                {
                    return false;
                }
            }

            if (searchTerms.Length != int.MinValue)
            {
                //only boats have length all others fail
                if (vehicle is not Boat)
                {
                    return false;
                }

                if ((vehicle as Boat).Length != searchTerms.Length)
                {
                    return false;
                }
            }

            //all terms matched or there were no terms to check hence everything matches
            return true;
        }
    }
}
