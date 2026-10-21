using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Garage.Vehicles
{
    internal class Car : Vehicle
    {
        internal enum Drive
        {
            Blank = -1,
            FWD,
            RWD,
            AWD,
            _4WD
        }

        private string regNum;
        internal Drive TypeOfDrive { get; set; }

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
                    throw new ArgumentException($"{value} is not a valid licence plate number for a car.");
                }

                regNum = value;
            }
        }

        internal Car(Drive drive, string registrationNumber, Color color, int numberOfWheels)
        {
            TypeOfDrive = drive;
            RegistrationNumber = registrationNumber;
            base.Color = color;
            base.NumberOfWheels = numberOfWheels;
        }
    }
}
