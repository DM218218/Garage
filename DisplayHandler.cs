using System;
using System.Collections.Generic;
using System.Text;

namespace Garage
{
    internal class DisplayHandler
    {
        private string printBuffer;
        private int maxColumnWidth;
        private int gutter;

        internal DisplayHandler(int displayWidth)
        {
            if (displayWidth < Program.MIN_DISPLAY_WIDTH)
            {
                throw new PlatformNotSupportedException($"Display width must be at least {Program.MIN_DISPLAY_WIDTH}");
            }

            gutter = 4;
            maxColumnWidth = (displayWidth - gutter) / 2;
            printBuffer = "";
        }

        internal void ShowHeader()
        {
            Console.WriteLine("***************************************");
            Console.WriteLine("*  Super Awesome Parking Application  *");
            Console.WriteLine("***************************************");
        }

        internal void ClearScreen()
        {
            Console.Clear();
        }

        internal void InsertLineBreak()
        {
            PrintLine("");
        }

        internal void PrintLine(string value)
        {
            Console.WriteLine(value);
        }

        internal bool StageLeft(string data)
        {
            printBuffer = "";

            if (data.Length > maxColumnWidth)
            {
                //we got a problem
                printBuffer = PadToRight("Data Width Issue");
                return false;
            }

            printBuffer = PadToRight(data);

            return true;
        }

        internal bool StageRight(string data)
        {
            if (data.Length > maxColumnWidth)
            {
                //we got a problem
                printBuffer += "Data Width Issue";
                return false;
            }

            printBuffer += data;

            return true;
        }

        private string PadToRight(string leftSection)
        {
            return leftSection.PadRight(maxColumnWidth + gutter);
        }

        internal void PrintBuffer()
        {
            PrintLine(printBuffer);
        }
    }
}
