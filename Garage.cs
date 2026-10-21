using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Garage
{
    internal class Garage
    {
        private Vehicle?[] parkingSpaces;
        private Random random;
        private int parkedVehicles;

        //Licence randomization data
        private string letters = "ABCDEFGHJKLMNOPRSTUWXYZ";
        private string position6Letters = "ABCDEFGHJKLMNPRSTUWXYZ";
        private string digits = "0123456789";
        private string digitsOver1 = "23456789";
        private string lettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string[] boatPrefixes;

        internal Vehicle[] ParkingSpaces { get => parkingSpaces; }
        internal int Capacity { get => parkingSpaces.Length; }

        internal Garage(int capacity)
        {
            random = new();
            parkingSpaces = new Vehicle[capacity];
            parkedVehicles = 0;
            InitBoatPrefixes();
        }

        private void InitBoatPrefixes()
        {
            boatPrefixes = new string[390]; //390 possible prefixes
            int mainIndex = 0;

            for (int a = 0; a < 13; a++)
            {
                for (int b = 0; b < 26; b++)
                {
                    boatPrefixes[mainIndex] = $"S{lettersAndDigits[a]}{lettersAndDigits[b]}";
                    mainIndex++;
                }
            }
                
            for (int c = 0; c < 26; c++)
            {
                boatPrefixes[mainIndex] = $"7S{lettersAndDigits[c]}";
                mainIndex++;
                boatPrefixes[mainIndex] = $"8S{lettersAndDigits[c]}";
                mainIndex++;
            }
        }

        internal bool PopulateGarage(int numberOfVehicles)
        {
            if(AvailableSpaces() < numberOfVehicles)
            {
                return false;
            }

            for(int i = 0; i < numberOfVehicles; i++)
            {
                ParkVehicle(GenerateRandomVehicle());
            }

            return true;
        }

        private Vehicle GenerateRandomVehicle()
        {
            Vehicle randomVehicle;
            int randomNumber = random.Next(4);

            if(randomNumber == 0)
            {
                //airplane
                randomVehicle = new Airplane(random.Next(6) + 3, GetRandomLicence(1), GetRandomColor(), random.Next(3) + 3);
            }
            else if (randomNumber == 1)
            {
                //boat
                randomVehicle = new Boat(random.Next(19) + 5, GetRandomLicence(2), GetRandomColor(), 0);
            }
            else if (randomNumber == 2)
            {
                //bus
                randomVehicle = new Bus(random.Next(32) + 8, GetRandomLicence(0), GetRandomColor(), random.Next(2) == 0 ? 4 : 6);
            }
            else if (randomNumber == 3)
            {
                //car
                randomVehicle = new Car(GetRandomDrive(), GetRandomLicence(0), GetRandomColor(), 4);
            }
            else if (randomNumber == 4)
            {
                //motorcycle
                randomVehicle = new Motorcycle(random.Next(450) + 550, GetRandomLicence(0), GetRandomColor(), random.Next(3) + 2);
            }
            else
            {
                throw new InvalidOperationException($"{randomNumber} is not a valid vehicle type.");
            }

            return randomVehicle;
        }

        private Car.Drive GetRandomDrive()
        {
            switch(random.Next(4))
            {
                case 0:
                    return Car.Drive.FWD;
                    //break;
                case 1:
                    return Car.Drive.RWD;
                    break;
                case 2:
                    return Car.Drive.AWD;
                    break;
                case 3:
                    return Car.Drive._4WD;
                    break;
                default:
                    return Car.Drive.FWD;
                    break;
            }
        }

        private string GetRandomLicence(int type)
        {
            var acceptable = false;
            var licence = string.Empty;

            if(type == 0)
            {
                //Car, Mc, Bus
                while(!acceptable)
                {
                    var sb = new StringBuilder();

                    sb.Append(letters[random.Next(letters.Length)]);
                    sb.Append(letters[random.Next(letters.Length)]);
                    sb.Append(letters[random.Next(letters.Length)]);
                    sb.Append(digits[random.Next(digits.Length)]);
                    sb.Append(digits[random.Next(digits.Length)]);

                    if(random.Next(2) == 0)
                    {
                        sb.Append(digits[random.Next(digits.Length)]);
                    }
                    else
                    {
                        sb.Append(position6Letters[random.Next(position6Letters.Length)]);
                    }

                    licence = sb.ToString();

                    acceptable = DMV.IsRegistrationNumberAvailable(licence, 0);
                }

                return licence;
            }
            else if(type == 1)
            {
                //airplane
                while (!acceptable)
                {
                    var sb = new StringBuilder("SE");

                    sb.Append(lettersAndDigits[random.Next(lettersAndDigits.Length)]);
                    sb.Append(lettersAndDigits[random.Next(lettersAndDigits.Length)]);
                    sb.Append(lettersAndDigits[random.Next(lettersAndDigits.Length)]);

                    licence = sb.ToString();

                    acceptable = DMV.IsRegistrationNumberAvailable(licence, 1);
                }

                return licence;
            }
            else if(type == 2)
            {
                //boat
                while (!acceptable)
                {
                    var sb = new StringBuilder();

                    sb.Append(boatPrefixes[random.Next(boatPrefixes.Length)]);
                    sb.Append(digitsOver1[random.Next(digitsOver1.Length)]);
                    sb.Append(digits[random.Next(digits.Length)]);
                    sb.Append(digits[random.Next(digits.Length)]);
                    sb.Append(digits[random.Next(digits.Length)]);

                    licence = sb.ToString();

                    acceptable = DMV.IsRegistrationNumberAvailable(licence, 2);
                }

                return licence;
            }
            else
            {
                throw new ArgumentException($"{type} is not a valid vehicle type.");
            }
        }

        private Color GetRandomColor()
        {
            switch (random.Next(4))
            {
                case 0:
                    return Color.Red;
                    break;
                case 1:
                    return Color.Black;
                    break;
                case 2:
                    return Color.Blue;
                    break;
                case 3:
                    return Color.White;
                    break;
                case 4:
                    return Color.Green;
                    break;
                default:
                    return Color.Pink;
                    break;
            }
        }

        internal bool ParkVehicle(Vehicle vehicle)
        {
            if(!HasSpaces())
            {
                return false;
            }

            if (DMV.IsRegistrationNumberAvailable(vehicle.RegistrationNumber,
                               vehicle is Car || vehicle is Bus || vehicle is Motorcycle ? 0 :
                               vehicle is Airplane ? 1 : 2))
            {

                for (int i = 0; i < parkingSpaces.Length; i++)
                {
                    if (parkingSpaces[i] is null)
                    {
                        parkingSpaces[i] = vehicle;
                        break;
                    }
                }
                DMV.RegisterNumber(vehicle.RegistrationNumber,
                                   vehicle is Car || vehicle is Bus || vehicle is Motorcycle ? 0 :
                                   vehicle is Airplane ? 1 : 2);
                parkedVehicles++;

                return true;
            }
            else
            {
                throw new ArgumentException($"{vehicle.RegistrationNumber} is already a registered vehicle.");
            }
        }

        private int GetIndex(string regNo)
        {
            return Array.FindIndex(parkingSpaces, veh =>
            {
                return veh is not null && string.Equals(veh.RegistrationNumber, regNo, StringComparison.OrdinalIgnoreCase);
            });
        }

        internal bool RemoveVehicle(string regNo)
        {
            return RemoveVehicle(GetIndex(regNo));
        }

        internal bool RemoveVehicle(int index)
        {
            if(index == -1)
            {
                return false;
            }

            if(parkingSpaces[index] is Car || parkingSpaces[index] is Bus || parkingSpaces[index] is Motorcycle)
            {
                DMV.DeregisterNumber(parkingSpaces[index].RegistrationNumber, 0);
            }
            else if(parkingSpaces[index] is Airplane)
            {
                DMV.DeregisterNumber(parkingSpaces[index].RegistrationNumber, 1);
            }
            else if(parkingSpaces[index] is Boat)
            {
                DMV.DeregisterNumber(parkingSpaces[index].RegistrationNumber, 2);
            }

            parkingSpaces[index] = null;

            parkedVehicles--;
            return true;
        }

        internal bool RemoveVehicle(Vehicle vehicle)
        {
            return RemoveVehicle(vehicle.RegistrationNumber);
        }

        internal bool HasSpaces()
        {
            return AvailableSpaces() > 0;
        }

        internal int AvailableSpaces()
        {
            return ParkingSpaces.Length - parkedVehicles;
        }

        internal void EmptyGarage()
        {
            //the garage is being removed. all vehicles must be deregistered.
            foreach(Vehicle vehicle in parkingSpaces)
            {
                //no need to keep track of anything just needs to free the licence numbers
                if(vehicle is Car || vehicle is Bus || vehicle is Motorcycle)
                {
                    DMV.DeregisterNumber(vehicle.RegistrationNumber, 0);
                }
                else if(vehicle is Airplane)
                {
                    DMV.DeregisterNumber(vehicle.RegistrationNumber, 1);
                }
                else if(vehicle is Boat)
                {
                    DMV.DeregisterNumber(vehicle.RegistrationNumber, 2);
                }
            }
        }
    }
}
