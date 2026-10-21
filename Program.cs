using System;

namespace Garage
{
    internal class Program
    {
        public static int MIN_DISPLAY_WIDTH = 70;

        static void Main(string[] args)
        {
            try
            {
                Menu menu = new(new(Console.BufferWidth), new());
                ParkingComplex complex = new();

                menu.Start(complex);
            }
            catch(PlatformNotSupportedException ex)
            {
                Console.WriteLine($"Error encountered: {ex.Message}");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }
    }
}
