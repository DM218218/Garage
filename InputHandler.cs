using System;
using System.Collections.Generic;
using System.Text;

namespace Garage
{
    internal class InputHandler
    {
        internal string GrabLine()
        {
            return Console.ReadLine();
        }

        internal string GrabKey()
        {
            return Console.ReadKey().Key.ToString();
        }
    }
}
