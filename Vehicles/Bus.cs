using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Garage.Vehicles
{
    internal class Bus : Vehicle
    {
        private string regNum;
        internal int NumberOfSeats { get; set; }

        internal override string RegistrationNumber
        {
            get => regNum;

            set
            {
                value = value.ToUpper();

                if (value.Length == 6 && DMV.IsValidCarMcBus(value))
                {
                    if (!DMV.IsRegistrationNumberAvailable(value, 0))
                    {
                        throw new ArgumentException($"{value} is already a registered vehicle.");
                    }
                }
                else
                {
                    throw new ArgumentException($"{value} is not a valid licence plate number for a bus.");
                }

                regNum = value;
            }
        }
        internal Bus(int numberOfSeats, string registrationNumber, Color color, int numberOfWheels)
        {
            NumberOfSeats = numberOfSeats;
            RegistrationNumber = registrationNumber;
            base.Color = color;
            base.NumberOfWheels = numberOfWheels;
        }
    }
}
