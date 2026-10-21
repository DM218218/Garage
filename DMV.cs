using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Garage
{
    internal static class DMV
    {
        private static string?[] registrationNumbersCarMcBus = new string[100];
        private static string?[] registrationNumbersAirplane = new string[100];
        private static string?[] registrationNumbersBoat = new string[100];
        private static int usedArraySlotsCarMcBus = 0;
        private static int usedArraySlotsAirplane = 0;
        private static int usedArraySlotsBoat = 0;

        public static bool IsRegistrationNumberAvailable(string number, int registry)
        {
            if (registry == 0)
            {
                return !Array.Exists(registrationNumbersCarMcBus, element => string.Equals(number, element, StringComparison.OrdinalIgnoreCase));
            }
            else if (registry == 1)
            {
                return !Array.Exists(registrationNumbersAirplane, element => string.Equals(number, element, StringComparison.OrdinalIgnoreCase));
            }
            else if (registry == 2)
            {
                return !Array.Exists(registrationNumbersBoat, element => string.Equals(number, element, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new ArgumentException($"Registry number {registry} is not a valid registry");
            }
        }

        public static void RegisterNumber(string number, int registry)
        {
            if (IsFull(registry))
            {
                GrowArray(registry);
            }

            if (registry == 0)
            {
                for (int i = 0; i < registrationNumbersCarMcBus.Length; i++)
                {
                    if (registrationNumbersCarMcBus[i] == null)
                    {
                        registrationNumbersCarMcBus[i] = number;
                        usedArraySlotsCarMcBus++;
                        break;
                    }
                }
            }
            else if (registry == 1)
            {
                for (int i = 0; i < registrationNumbersAirplane.Length; i++)
                {
                    if (registrationNumbersAirplane[i] == null)
                    {
                        registrationNumbersAirplane[i] = number;
                        usedArraySlotsAirplane++;
                        break;
                    }
                }
            }
            else if (registry == 2)
            {
                for (int i = 0; i < registrationNumbersBoat.Length; i++)
                {
                    if (registrationNumbersBoat[i] == null)
                    {
                        registrationNumbersBoat[i] = number;
                        usedArraySlotsBoat++;
                        break;
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Registry number {registry} is not a valid registry");
            }
        }

        public static void DeregisterNumber(string number, int registry)
        {
            if (registry == 0)
            {
                for (int i = 0; i < registrationNumbersCarMcBus.Length; i++)
                {
                    if (string.Equals(number, registrationNumbersCarMcBus[i], StringComparison.OrdinalIgnoreCase))
                    {
                        registrationNumbersCarMcBus[i] = null;
                        usedArraySlotsCarMcBus--;
                        break;
                    }
                }
            }
            else if (registry == 1)
            {
                for (int i = 0; i < registrationNumbersAirplane.Length; i++)
                {
                    if (string.Equals(number, registrationNumbersAirplane[i], StringComparison.OrdinalIgnoreCase))
                    {
                        registrationNumbersAirplane[i] = null;
                        usedArraySlotsAirplane--;
                        break;
                    }
                }
            }
            else if (registry == 2)
            {
                for (int i = 0; i < registrationNumbersBoat.Length; i++)
                {
                    if (string.Equals(number, registrationNumbersBoat[i], StringComparison.OrdinalIgnoreCase))
                    {
                        registrationNumbersBoat[i] = null;
                        usedArraySlotsBoat--;
                        break;
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Registry number {registry} is not a valid registry");
            }
        }

        private static void GrowArray(int registry)
        {
            if (registry == 0)
            {
                Array.Resize(ref registrationNumbersCarMcBus, registrationNumbersCarMcBus.Length * 2);
            }
            else if (registry == 1)
            {
                Array.Resize(ref registrationNumbersAirplane, registrationNumbersAirplane.Length * 2);
            }
            else if (registry == 2)
            {
                Array.Resize(ref registrationNumbersBoat, registrationNumbersBoat.Length * 2);
            }
            else
            {
                throw new ArgumentException($"Registry number {registry} is not a valid registry");
            }
        }

        private static bool IsFull(int registry)
        {
            if (registry == 0)
            {
                return registrationNumbersCarMcBus.Length <= usedArraySlotsCarMcBus;
            }
            else if (registry == 1)
            {
                return registrationNumbersAirplane.Length <= usedArraySlotsAirplane;
            }
            else if (registry == 2)
            {
                return registrationNumbersBoat.Length <= usedArraySlotsBoat;
            }
            else
            {
                throw new ArgumentException($"Registry number {registry} is not a valid registry");
            }
        }

        internal static bool IsValidCarMcBus(string value)
        {
            value = value.ToUpper();

            // Positions 1-3: A-Z except I, Q, and V
            string letters = "[A-HJ-PR-UW-Z]";

            // Position 6 when it's a letter: A-Z but except I, Q, V, and O
            string lastLetter = "[A-HJ-NPR-UW-Z]";

            // ABC123 or ABC12D
            Regex expression = new($"^{letters}{{3}}\\d{{2}}(\\d|{lastLetter})$", RegexOptions.Compiled);

            return expression.IsMatch(value);
        }

        internal static bool IsValidAirplane(string value)
        {
            // SE + three characters (letters and/or digits).
            Regex expression = new(@"^SE[A-Z0-9]{3}$", RegexOptions.Compiled);

            return expression.IsMatch(value);
        }

        internal static bool IsValidBoat(string value)
        {
            // Call sign for boats (<= 24 m): prefix from SAA-SMZ, 7SA-7SZ or 8SA-8SZ.
            string Prefix = "(?:S[A-M][A-Z]|[78]S[A-Z])";

            // After prefix should be four digits, first digit must be 2 or higher
            Regex expression = new($"^{Prefix}[2-9]\\d{{3}}$", RegexOptions.Compiled);

            return expression.IsMatch(value);
        }
    }
}
