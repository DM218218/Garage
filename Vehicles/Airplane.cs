using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Garage.Vehicles
{
    internal class Airplane : Vehicle
    {
        private string regNum;
        internal int Wingspan { get; set; }

        internal override string RegistrationNumber
        {
            get => regNum;

            set
            {
                value = value.ToUpper();

                if (value.Length == 5 && DMV.IsValidAirplane(value))
                {
                    if (!DMV.IsRegistrationNumberAvailable(value, 1))
                    {
                        throw new ArgumentException($"{value} is already a registered airplane.");
                    }
                }
                else
                {
                    throw new ArgumentException($"{value} is not a valid licence plate number for a airplane.");
                }

                regNum = value;
            }
        }

        internal Airplane(int wingspan, string registrationNumber, Color color, int numberOfWheels)
        {
            Wingspan = wingspan;
            RegistrationNumber = registrationNumber;
            base.Color = color;
            base.NumberOfWheels = numberOfWheels;
        }
    }
}
