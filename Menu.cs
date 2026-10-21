using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;

namespace Garage
{
    internal class Menu
    {
        private enum MenuState
        {
            StartDisplay,       //garage list
            VehiclesDisplay,    //vehicles list
            TypeCountDisplay,   //list of number of vehicle types
            SearchDisplay       //Enter search term and list result
        }

        private DisplayHandler displayHandler;
        private InputHandler inputHandler;
        private MenuState menuState;
        private ParkingComplex complex;

        private string[] commands;

        public Menu(DisplayHandler dh, InputHandler ih)
        {
            displayHandler = dh;
            inputHandler = ih;

            initAndPopulateCommands();
        }

        private void initAndPopulateCommands()
        {
            commands = new string[9];

            commands[0] = "1. List vehicles in garages";
            commands[1] = "2. List vehicle types in garages";
            commands[2] = "3. Add vehicle to garage";
            commands[3] = "4. Remove vehicle from garage";
            commands[4] = "5. Search vehicle";
            commands[5] = "6. Create new garage";
            commands[6] = "7. Remove garage";
            commands[7] = "8. Populate garage";
            commands[8] = "0. Quit";
        }

        internal void Start(ParkingComplex complex)
        {
            this.complex = complex;
            menuState = MenuState.StartDisplay;

            ShowMenu();
        }

        internal void ShowMenu()
        {
            bool run = true;

            while (run)
            {
                displayHandler.ShowHeader();

                switch (menuState)
                {
                    case MenuState.StartDisplay:
                        {
                            var garages = complex.GetAllGarages();
                            int i = 0;

                            //show left column data
                            for (; i < garages.Length; i++)
                            {
                                string leftPart = $"Garage {i + 1}: cap {garages[i].Capacity}, free {garages[i].AvailableSpaces()}";
                                string rightPart = i < commands.Length ? commands[i] : "";
                                ShowLine(leftPart, rightPart);
                            }

                            //show right column commands if more than data
                            for (; i < commands.Length; i++)
                            {
                                string leftPart = "";
                                string rightPart = i < commands.Length ? commands[i] : "";
                                ShowLine(leftPart, rightPart);
                            }

                            displayHandler.InsertLineBreak();
                            displayHandler.InsertLineBreak();
                            displayHandler.PrintLine("Pick an option. Confirm with Enter.");
                            int command = -1;

                            while (!int.TryParse(inputHandler.GrabLine(), out command))
                            {
                                displayHandler.PrintLine("Pick an option. Confirm with Enter.");
                            }

                            run = ProcessInput(command);
                            break;
                        }
                    case MenuState.VehiclesDisplay:
                        {
                            var garages = complex.GetAllGarages();

                            foreach (Garage garage in garages)
                            {
                                DisplayVehicleArray(garage.ParkingSpaces);
                            }

                            displayHandler.InsertLineBreak();
                            displayHandler.PrintLine("Press any key to return to main menu.");
                            inputHandler.GrabKey();

                            run = ProcessInput(-1);
                            break;
                        }
                    case MenuState.TypeCountDisplay:
                        {
                            DisplayTypeCount();

                            displayHandler.InsertLineBreak();
                            displayHandler.PrintLine("Press any key to return to main menu.");
                            inputHandler.GrabKey();

                            run = ProcessInput(-1);
                            break;
                        }
                    case MenuState.SearchDisplay:
                        {
                            string query = GetSearchTerm();

                            var result = ProcessSearch(query);

                            DisplaySearchResult(result);

                            displayHandler.InsertLineBreak();
                            displayHandler.PrintLine("Press any key to return to main menu.");
                            inputHandler.GrabKey();

                            run = ProcessInput(-1);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private void DisplayVehicleArray(Vehicle[] vehicleList)
        {
            foreach (Vehicle vehicle in vehicleList)
            {
                if (vehicle is not null)
                {
                    if (vehicle is Car)
                    {
                        Car car = vehicle as Car;
                        StringBuilder data = new("Car");
                        data.Append(", ");
                        data.Append(car.RegistrationNumber);
                        data.Append(", ");
                        data.Append(car.Color.Name);
                        data.Append(", ");
                        data.Append("wheels:" + car.NumberOfWheels);
                        data.Append(", ");
                        data.Append("drive type:");
                        data.Append(car.TypeOfDrive == Car.Drive.FWD ? "FWD" :
                                    car.TypeOfDrive == Car.Drive.RWD ? "RWD" :
                                    car.TypeOfDrive == Car.Drive.AWD ? "AWD" : "4WD");

                        displayHandler.PrintLine(data.ToString());
                    }
                    else if (vehicle is Bus)
                    {
                        Bus bus = vehicle as Bus;
                        StringBuilder data = new("Bus");
                        data.Append(", ");
                        data.Append(bus.RegistrationNumber);
                        data.Append(", ");
                        data.Append(bus.Color.Name);
                        data.Append(", ");
                        data.Append("wheels:" + bus.NumberOfWheels);
                        data.Append(", ");
                        data.Append("seats:" + bus.NumberOfSeats);

                        displayHandler.PrintLine(data.ToString());
                    }
                    else if (vehicle is Motorcycle)
                    {
                        Motorcycle mc = vehicle as Motorcycle;
                        StringBuilder data = new("MC");
                        data.Append(", ");
                        data.Append(mc.RegistrationNumber);
                        data.Append(", ");
                        data.Append(mc.Color.Name);
                        data.Append(", ");
                        data.Append("wheels:" + mc.NumberOfWheels);
                        data.Append(", ");
                        data.Append("seat heigth:" + mc.SeatHeight);

                        displayHandler.PrintLine(data.ToString());
                    }
                    else if (vehicle is Airplane)
                    {
                        Airplane plane = vehicle as Airplane;
                        StringBuilder data = new("Airplane");
                        data.Append(", ");
                        data.Append(plane.RegistrationNumber);
                        data.Append(", ");
                        data.Append(plane.Color.Name);
                        data.Append(", ");
                        data.Append("wheels:" + plane.NumberOfWheels);
                        data.Append(", ");
                        data.Append("wingspan:" + plane.Wingspan);

                        displayHandler.PrintLine(data.ToString());
                    }
                    else if (vehicle is Boat)
                    {
                        Boat boat = vehicle as Boat;
                        StringBuilder data = new("Boat");
                        data.Append(", ");
                        data.Append(boat.RegistrationNumber);
                        data.Append(", ");
                        data.Append(boat.Color.Name);
                        data.Append(", ");
                        data.Append("wheels:" + boat.NumberOfWheels);
                        data.Append(", ");
                        data.Append("length:" + boat.Length);

                        displayHandler.PrintLine(data.ToString());
                    }
                }
            }
        }

        private void DisplaySearchResult(Vehicle[] result)
        {
            displayHandler.ClearScreen();
            displayHandler.ShowHeader();

            if(result.Length <= 0)
            {
                displayHandler.PrintLine("No vehicle matching the search was found.");
            }
            else
            {
                DisplayVehicleArray(result);
            }
        }

        private Vehicle[] ProcessSearch(string query)
        {
            query.ToLower();
            var terms = query.Split("-");

            ParkingComplex.SearchTerms searchTerms = new();

            foreach(var term in terms)
            {
                // to short to contain anything
                if(term.Length < 3)
                {
                    continue;
                }

                //checking first character in term
                switch(term[0])
                {
                    case 't':
                        searchTerms.Type = term.Substring(2).ToLower().Trim();
                        break;
                    case 'c':
                        string colorTerm = term.Substring(2).ToLower().Trim();
                        searchTerms.Color = colorTerm == "black" ? Color.Black :
                                            colorTerm == "blue" ? Color.Blue :
                                            colorTerm == "white" ? Color.White :
                                            colorTerm == "red" ? Color.Red :
                                            colorTerm == "green" ? Color.Green :
                                            colorTerm == "pink" ? Color.Pink :
                                            Color.Empty;
                        break;
                    case 'l':
                        searchTerms.Licence = term.Substring(2).ToLower().Trim();
                        break;
                    case 'd':
                        string driveTerm = term.Substring(2).ToLower().Trim();
                        searchTerms.DriveType = driveTerm == "fwd" ? Car.Drive.FWD :
                                                driveTerm == "rwd" ? Car.Drive.RWD :
                                                driveTerm == "awd" ? Car.Drive.AWD :
                                                driveTerm == "4wd" ? Car.Drive._4WD :
                                                Car.Drive.Blank;
                        break;
                    case 'w':
                        string wheelTerm = term.Substring(2);
                        int wheels = int.MinValue;
                        int.TryParse(wheelTerm, out wheels);
                        searchTerms.NumberOfWheels = wheels;
                        break;
                    case 'h':
                        string heightTerm = term.Substring(2);
                        int seatHeight = int.MinValue;
                        int.TryParse(heightTerm, out seatHeight);
                        searchTerms.SeatHeight = seatHeight;
                        break;
                    case 's':
                        string seatTerm = term.Substring(2);
                        int seats = int.MinValue;
                        int.TryParse(seatTerm, out seats);
                        searchTerms.NumberOfSeats = seats;
                        break;
                    case 'i':
                        string wingTerm = term.Substring(2);
                        int wingspan = int.MinValue;
                        int.TryParse(wingTerm, out wingspan);
                        searchTerms.Wingspan = wingspan;
                        break;
                    case 'e':
                        string lengthTerm = term.Substring(2);
                        int length = int.MinValue;
                        int.TryParse(lengthTerm, out length);
                        searchTerms.Length = length;
                        break;
                    default:
                        break;
                }
            }

            return complex.SearchGarages(searchTerms);
        }

        private string GetSearchTerm()
        {
            displayHandler.PrintLine("Search syntax, enter term without brackets ex: '-t car -c blue'");
            displayHandler.PrintLine("-t [Vehicle type] - valid values: car, motorcycle, bus, airplane, boat");
            displayHandler.PrintLine("-c [Color] - valid values: black, red, green, blue, white, pink");
            displayHandler.PrintLine("-l [Licence number] - valid values: Depends on vehicle type formatting");
            displayHandler.PrintLine("-w [Number of wheels] - valid values: integer");
            displayHandler.PrintLine("-d [Drive type] - cars only - valid values: FWD, RWD, AWD, 4WD");
            displayHandler.PrintLine("-h [Seat height] - motorcycles only - valid values: integer");
            displayHandler.PrintLine("-s [Number of seats] - buses only - valid values: integer");
            displayHandler.PrintLine("-i [Wingspan] - airplanes only - valid values: integer");
            displayHandler.PrintLine("-e [Length] - boats only - valid values: integer");
            displayHandler.PrintLine("Terms are treated as and conditions. Only one value per term is accepted.");
            displayHandler.PrintLine("Please enter a search query. Blank terms will be ignored.");
            displayHandler.InsertLineBreak();

            return inputHandler.GrabLine();
        }

        private void DisplayTypeCount()
        {
            var garages = complex.GetAllGarages();
            var sb = new StringBuilder();
            int cars = 0;
            int planes = 0;
            int buses = 0;
            int mcs = 0;
            int boats = 0;

            foreach (Garage garage in garages)
            {
                foreach (Vehicle vehicle in garage.ParkingSpaces)
                {
                    if (vehicle is Car)
                    {
                        cars++;
                    }
                    else if (vehicle is Airplane)
                    {
                        planes++;
                    }
                    else if (vehicle is Bus)
                    {
                        buses++;
                    }
                    else if (vehicle is Motorcycle)
                    {
                        mcs++;
                    }
                    else if (vehicle is Boat)
                    {
                        boats++;
                    }
                }
            }

            displayHandler.PrintLine("The garages contain:");
            displayHandler.PrintLine($"{cars} Cars");
            displayHandler.PrintLine($"{mcs} Motorcycles");
            displayHandler.PrintLine($"{buses} Buses");
            displayHandler.PrintLine($"{planes} Airplanes");
            displayHandler.PrintLine($"{boats} Boats");
        }

        private bool ProcessInput(int command)
        {
            switch (menuState)
            {
                case MenuState.StartDisplay:
                    if (command == 1)
                    {
                        //List vehicles
                        menuState = MenuState.VehiclesDisplay;
                        displayHandler.ClearScreen();
                    }
                    else if (command == 2)
                    {
                        //list type count
                        menuState = MenuState.TypeCountDisplay;
                        displayHandler.ClearScreen();
                    }
                    else if (command == 3)
                    {
                        //park vehicle
                        ParkVehicle();

                        inputHandler.GrabKey();
                        displayHandler.ClearScreen();
                    }
                    else if (command == 4)
                    {
                        //remove vehicle
                        displayHandler.PrintLine("Enter licence number of vehicle to remove. Confirm with Enter");

                        string licence = inputHandler.GrabLine();

                        if (DMV.IsValidCarMcBus(licence) || DMV.IsValidAirplane(licence) || DMV.IsValidBoat(licence))
                        {
                            if (complex.RemoveVehicle(licence))
                            {
                                displayHandler.PrintLine("Vehicle removed. Press any key to continue.");
                            }
                            else
                            {
                                displayHandler.PrintLine("Vehicle not found. Press any key to continue.");
                            }
                        }
                        else
                        {
                            displayHandler.PrintLine($"{licence} is not a valid vehicle licence number. Press any key to continue.");
                        }

                        inputHandler.GrabKey();
                        displayHandler.ClearScreen();
                    }
                    else if (command == 5)
                    {
                        //search

                        menuState = MenuState.SearchDisplay;
                        displayHandler.ClearScreen();
                    }
                    else if (command == 6)
                    {
                        //create garage
                        displayHandler.PrintLine("Enter garage capacity, Enter to confirm.");

                        int capacity = 0;

                        while (!int.TryParse(inputHandler.GrabLine(), out capacity))
                        {
                            displayHandler.PrintLine("Enter garage capacity, Enter to confirm.");
                        }

                        complex.CreateGarage(capacity);

                        displayHandler.PrintLine("Garage created. Press any key to continue.");

                        inputHandler.GrabKey();
                        displayHandler.ClearScreen();
                    }
                    else if (command == 7)
                    {
                        //remove garage
                        displayHandler.PrintLine("Choose a garage, it and all vehicles in it will be removed! Enter to confirm choose 0 to cancel.");
                        int garageNumber = 0;
                        if (int.TryParse(inputHandler.GrabLine(), out garageNumber))
                        {
                            if (garageNumber > complex.GetAllGarages().Length || garageNumber <= 0)
                            {
                                displayHandler.PrintLine("No such garage. Press any key to continue.");
                                inputHandler.GrabKey();
                                displayHandler.ClearScreen();
                            }
                            else
                            {
                                try
                                {
                                    complex.RemoveGarage(garageNumber - 1);

                                    displayHandler.PrintLine("Garage removed. Press any key to continue.");
                                }
                                catch (IndexOutOfRangeException ex)
                                {
                                    displayHandler.PrintLine(ex.Message);
                                    displayHandler.PrintLine("Press any key to continue.");
                                }
                                inputHandler.GrabKey();
                                displayHandler.ClearScreen();
                            }
                        }
                    }
                    else if (command == 8)
                    {
                        //populate garage
                        displayHandler.PrintLine("Choose a garage, Enter to confirm.");
                        int garageNumber = 0;
                        if (int.TryParse(inputHandler.GrabLine(), out garageNumber))
                        {
                            if (garageNumber > complex.GetAllGarages().Length || garageNumber <= 0)
                            {
                                displayHandler.PrintLine("No such garage. Press any key to continue.");
                                inputHandler.GrabKey();
                                displayHandler.ClearScreen();
                            }

                            int numberOfVehicles = 0;

                            displayHandler.PrintLine("How many vehicles to create? Enter to confirm.");

                            while (!int.TryParse(inputHandler.GrabLine(), out numberOfVehicles))
                            {
                                displayHandler.PrintLine("How many vehicles to create? Enter to confirm.");
                            }

                            if (!complex.GetGarage(garageNumber - 1).PopulateGarage(numberOfVehicles))
                            {
                                displayHandler.PrintLine("Not able to fit that many vehicles. Press any key to continue.");
                            }
                            else
                            {
                                displayHandler.PrintLine("Garage populated. Press any key to continue.");
                            }
                        }

                        inputHandler.GrabKey();
                        displayHandler.ClearScreen();
                    }
                    else if (command == 0)
                    {
                        //quitting program
                        return false;
                    }
                    break;
                case MenuState.VehiclesDisplay:
                case MenuState.TypeCountDisplay:
                case MenuState.SearchDisplay:
                    if (command == -1)
                    {
                        menuState = MenuState.StartDisplay;
                        displayHandler.ClearScreen();
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private void ParkVehicle()
        {
            int vehicleType = PickAVehicle();

            if(vehicleType == -1)
            {
                return;
            }

            Vehicle vehicle = MakeVehicle(vehicleType);

            if (vehicle is not null)
            {
                int garage = -1;

                displayHandler.PrintLine("Pick a garage to park in. Confirm with Enter.");

                if (int.TryParse(inputHandler.GrabLine(), out garage))
                {
                    if (garage > 0 && garage <= complex.GetAllGarages().Length)
                    {
                        try
                        {
                            if (complex.ParkVehicle(vehicle, garage - 1))
                            {
                                displayHandler.PrintLine("Vehicle has been parked. Press any key to continue.");
                            }
                            else
                            {
                                displayHandler.PrintLine("There is no space in the garage. Press any key to continue.");
                            }
                        }
                        catch(ArgumentException ex)
                        {
                            displayHandler.PrintLine(ex.Message);
                            displayHandler.PrintLine("Press any key to continue.");
                        }
                    }
                    else
                    {
                        displayHandler.PrintLine("That is not a valid option. Press any key to continue.");
                    }
                }
                else
                {
                    displayHandler.PrintLine("That is not a valid option. Press any key to continue.");
                }
            }
        }

        private Vehicle MakeVehicle(int vehicleType)
        {
            string licence;
            Color color;
            int wheels;

            displayHandler.PrintLine("Pick a color, confirm with enter.");
            displayHandler.PrintLine("1. Blue, 2. Red, 3. Black, 4. Green, 5. White, 6. Pink");
            int colorInput = -1;

            if (int.TryParse(inputHandler.GrabLine(), out colorInput))
            {
                //is the choice not valid
                if (colorInput <= 0 && colorInput >= 7)
                {
                    displayHandler.PrintLine($"That is not a valid color option. Press any key to continue.");
                    return null;
                }

                color = colorInput == 1 ? Color.Blue :
                        colorInput == 2 ? Color.Red :
                        colorInput == 3 ? Color.Black :
                        colorInput == 4 ? Color.Green :
                        colorInput == 5 ? Color.White : Color.Pink;
            }
            else
            {
                displayHandler.PrintLine($"That is not a valid color option. Press any key to continue.");
                return null;
            }

            displayHandler.PrintLine("Please enter number of wheels, confirm with enter");

            if (!int.TryParse(inputHandler.GrabLine(), out wheels))
            {
                displayHandler.PrintLine($"That is not a valid color option. Press any key to continue.");
                return null;
            }

            if (vehicleType == 1)
            {
                //car
                displayHandler.PrintLine("Enter the licence plate number of the car.");
                displayHandler.PrintLine("Three letters, excluding I, Q and, V followed by two numbers followed by " +
                                         "a third number or a letter excluding I, Q, V, and O.");
                displayHandler.PrintLine("Confirm with Enter.");
                licence = inputHandler.GrabLine();

                if (!DMV.IsValidCarMcBus(licence))
                {
                    displayHandler.PrintLine($"{licence} is not a valid car licence number. Press any key to continue.");
                    return null;
                }

                displayHandler.PrintLine("Pick drive type, confirm with Enter.");
                displayHandler.PrintLine("1. FWB, 2. RWD, 3. AWD, 4. 4WD");
                int driveType = -1;

                if (int.TryParse(inputHandler.GrabLine(), out driveType))
                {
                    if (driveType <= 0 && driveType >= 5)
                    {
                        displayHandler.PrintLine($"That is not a valid option. Press any key to continue.");
                        return null;
                    }
                }
                else
                {
                    displayHandler.PrintLine($"That is not a valid option. Press any key to continue.");
                    return null;
                }

                try
                {
                    return new Car(driveType == 1 ? Car.Drive.FWD :
                                   driveType == 2 ? Car.Drive.RWD :
                                   driveType == 3 ? Car.Drive.AWD : Car.Drive._4WD, licence, color, wheels);
                }
                catch (ArgumentException ex)
                {
                    displayHandler.PrintLine(ex.Message);
                    displayHandler.PrintLine("Press any key to continue.");
                    return null;
                }
            }
            else if (vehicleType == 2)
            {
                //mc
                displayHandler.PrintLine("Enter the licence plate number of the motorcycle.");
                displayHandler.PrintLine("Three letters, excluding I, Q and, V followed by two numbers followed by " +
                                         "a third number or a letter excluding I, Q, V, and O.");
                displayHandler.PrintLine("Confirm with Enter.");
                licence = inputHandler.GrabLine();

                if (!DMV.IsValidCarMcBus(licence))
                {
                    displayHandler.PrintLine($"{licence} is not a valid motorcycle licence number. Press any key to continue.");
                    return null;
                }

                displayHandler.PrintLine("Enter seat height in mm, confirm with Enter.");
                int seatHeight = -1;

                if (int.TryParse(inputHandler.GrabLine(), out seatHeight))
                {
                    if (seatHeight < 0)
                    {
                        displayHandler.PrintLine($"Seat height can not be less than zero. Press any key to continue.");
                        return null;
                    }
                }
                else
                {
                    displayHandler.PrintLine($"Seat height must be a number. Press any key to continue.");
                    return null;
                }

                try
                {
                    return new Motorcycle(seatHeight, licence, color, wheels);
                }
                catch (ArgumentException ex)
                {
                    displayHandler.PrintLine(ex.Message);
                    displayHandler.PrintLine("Press any key to continue.");
                    return null;
                }
            }
            else if (vehicleType == 3)
            {
                //bus
                displayHandler.PrintLine("Enter the licence plate number of the bus.");
                displayHandler.PrintLine("Three letters, excluding I, Q and, V followed by two numbers followed by " +
                                         "a third number or a letter excluding I, Q, V, and O.");
                displayHandler.PrintLine("Confirm with Enter.");
                licence = inputHandler.GrabLine();

                if (!DMV.IsValidCarMcBus(licence))
                {
                    displayHandler.PrintLine($"{licence} is not a valid bus licence number. Press any key to continue.");
                    return null;
                }

                displayHandler.PrintLine("Enter number of seats, confirm with Enter.");
                int seats = -1;

                if (int.TryParse(inputHandler.GrabLine(), out seats))
                {
                    if (seats <= 0)
                    {
                        displayHandler.PrintLine($"Must have at least one seat. Press any key to continue.");
                        return null;
                    }
                }
                else
                {
                    displayHandler.PrintLine($"Number of seats must be a number. Press any key to continue.");
                    return null;
                }

                try
                {
                    return new Bus(seats, licence, color, wheels);
                }
                catch (ArgumentException ex)
                {
                    displayHandler.PrintLine(ex.Message);
                    displayHandler.PrintLine("Press any key to continue.");
                    return null;
                }
            }
            else if (vehicleType == 4)
            {
                //plane
                displayHandler.PrintLine("Enter the licence plate number of the airplane.");
                displayHandler.PrintLine("'SE' followed by three letters or numbers.");
                displayHandler.PrintLine("Confirm with Enter.");
                licence = inputHandler.GrabLine();

                if (!DMV.IsValidAirplane(licence))
                {
                    displayHandler.PrintLine($"{licence} is not a valid airplane licence number. Press any key to continue.");
                    return null;
                }

                displayHandler.PrintLine("Enter wingspan, confirm with Enter.");
                int wingspan = -1;

                if (int.TryParse(inputHandler.GrabLine(), out wingspan))
                {
                    if (wingspan <= 0)
                    {
                        displayHandler.PrintLine($"Wingspan must be more than zero. Press any key to continue.");
                        return null;
                    }
                }
                else
                {
                    displayHandler.PrintLine($"Wingspan must be a number. Press any key to continue.");
                    return null;
                }

                try
                {
                    return new Airplane(wingspan, licence, color, wheels);
                }
                catch (ArgumentException ex)
                {
                    displayHandler.PrintLine(ex.Message);
                    displayHandler.PrintLine("Press any key to continue.");
                    return null;
                }
            }
            else if (vehicleType == 5)
            {
                //boat
                displayHandler.PrintLine("Enter the licence plate number of the boat.");
                displayHandler.PrintLine("A prefix in the ranges SAA-SMZ, 7SA-7SZ or 8SA-8SZ,");
                displayHandler.PrintLine("followed by four digits, the first must be 2 or higher.");
                displayHandler.PrintLine("Confirm with Enter.");
                licence = inputHandler.GrabLine();

                if (!DMV.IsValidBoat(licence))
                {
                    displayHandler.PrintLine($"{licence} is not a valid boat licence number. Press any key to continue.");
                    return null;
                }

                displayHandler.PrintLine("Enter boat length, confirm with Enter.");
                int length = -1;

                if (int.TryParse(inputHandler.GrabLine(), out length))
                {
                    if (length < 5 || length > 24)
                    {
                        displayHandler.PrintLine("Boats must be 5 meters or longer to be registered. Vessels over 24 meters are ships.");
                        displayHandler.PrintLine("Press any key to continue.");
                        return null;
                    }
                }
                else
                {
                    displayHandler.PrintLine($"Length must be a number. Press any key to continue.");
                    return null;
                }

                try
                {
                    return new Boat(length, licence, color, wheels);
                }
                catch (ArgumentException ex)
                {
                    displayHandler.PrintLine(ex.Message);
                    displayHandler.PrintLine("Press any key to continue.");
                    return null;
                }
            }

            return null;
        }

        private int PickAVehicle()
        {
            int type = -1;

            displayHandler.PrintLine("1. Car");
            displayHandler.PrintLine("2. Motorcycle");
            displayHandler.PrintLine("3. Bus");
            displayHandler.PrintLine("4. Airplane");
            displayHandler.PrintLine("5. Boat");
            displayHandler.InsertLineBreak();
            displayHandler.PrintLine("Please pick a vehicle type. Confirm with Enter.");

            if (int.TryParse(inputHandler.GrabLine(), out type))
            {
                if (type > 0 && type < 6)
                {
                    return type;
                }
                else
                {
                    type = -1;
                }
            }
            else
            {
                type = -1;
            }

            displayHandler.PrintLine("That is not a valid vehicle type. Press any key to continue.");
            return type;
        }

        private void ShowLine(string leftPart, string rightPart)
        {
            displayHandler.StageLeft(leftPart);
            displayHandler.StageRight(rightPart);
            displayHandler.PrintBuffer();
        }
    }
}
