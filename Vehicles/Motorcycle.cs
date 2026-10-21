using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Garage.Vehicles
{
    internal class Motorcycle : Vehicle
    {
        private string regNum;
        internal int SeatHeight { get; set; }

        internal override string RegistrationNumber 
        { 
            get => regNum; 
            
            set
            {
                value = value.ToUpper();

                if (value.Length == 6 && DMV.IsValidCarMcBus(value))
                {
                    if(!DMV.IsRegistrationNumberAvailable(value, 0))
                    {
                        throw new ArgumentException($"{value} is already a registered vehicle.");
                    }
                }
                else
                {
                    throw new ArgumentException($"{value} is not a valid licence plate number for a motorcycle.");
                }

                regNum = value;
            }
        }

        internal Motorcycle(int seatHeight, string registrationNumber, Color color, int numberOfWheels)
        {
            base.NumberOfWheels = numberOfWheels;
            base.Color = color;
            RegistrationNumber = registrationNumber;
            SeatHeight = seatHeight;
        }
    }
}
