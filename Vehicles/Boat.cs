using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Garage.Vehicles
{
    internal class Boat : Vehicle
    {
        private string regNum;
        internal int Length { get; set; }

        internal override string RegistrationNumber
        {
            get => regNum;

            set
            {
                value = value.ToUpper();

                if (value.Length == 7 && DMV.IsValidBoat(value))
                {
                    if (!DMV.IsRegistrationNumberAvailable(value, 2))
                    {
                        throw new ArgumentException($"{value} is already a registered boat.");
                    }
                }
                else
                {
                    throw new ArgumentException($"{value} is not a valid licence plate number for a boat.");
                }

                regNum = value;
            }
        }
        internal Boat(int length, string registrationNumber, Color color, int numberOfWheels)
        {
            Length = length;

            if(Length < 5 || Length > 24)
            {
                throw new ArgumentException("A registrable boat must be 5 meters or longer. Above 24 meters long is a ship.");
            }

            RegistrationNumber = registrationNumber;
            base.Color = color;
            base.NumberOfWheels = numberOfWheels;
        }
    }
}
