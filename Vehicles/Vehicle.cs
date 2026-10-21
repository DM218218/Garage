using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Garage.Vehicles
{
    internal class Vehicle
    {
        internal virtual string RegistrationNumber { get; set; }
        internal Color Color { get; set; }
        internal int NumberOfWheels { get; set; }

        protected Vehicle()
        {

        }
    }
}
