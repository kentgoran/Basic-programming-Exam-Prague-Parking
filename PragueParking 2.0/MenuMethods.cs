using System;
using System.Collections.Generic;
using System.IO;

namespace PragueParking_2._0
{
    class MenuMethods
    {
        private List<Vehicle> pLot = new List<Vehicle>();
        public void MainMenu()
        {
            ReadWrite rw = new ReadWrite();
            try
            {
                pLot = rw.ReadDatabase();
            }
            catch (FileNotFoundException ex)
            {
                
                Console.WriteLine(ex.Message);
                Console.Write("You have two options now. Create a new, empty database, OR close the program and find the file.\nDo you want to create a new, empty database? This will remove any parked cars from the system!\n[Y]es or [N]o: ");
                ConsoleKey input = Console.ReadKey().Key;
                Console.WriteLine();
                if(input == ConsoleKey.Y)
                {
                    FileStream tempStream = File.Create("database.txt");
                    tempStream.Close();
                    for(int i=0; i<200; i++)
                    {
                        pLot.Add(new Vehicle(Vehicle.VehicleType.EMPTY, "EMPTY", DateTime.MinValue));
                    }
                    rw.WriteDatabase(pLot);
                    Console.WriteLine("New empty file 'database.txt' created. Any parked cars are gone from the system.");
                    Console.ReadLine();
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }         
            while (true)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Prague Parking, version 2.0");
                Console.WriteLine("***************************");
                Console.WriteLine("[1]    Add a vehicle");
                Console.WriteLine("[2]    Manually move a vehicle to another spot");
                Console.WriteLine("[3]    Remove vehicle");
                Console.WriteLine("[4]    Search for a vehicle using reg number");
                Console.WriteLine("[5]    Check a specific parking spot");
                Console.WriteLine("[6]    Optimize motorcycle parking");
                Console.WriteLine("[7]    Re-arrange parking to fill from spot 1 and onwards");
                Console.WriteLine("[8]    Parking lot overview");
                Console.WriteLine("[9]    Exit");
                ConsoleKey input = Console.ReadKey().Key;
                switch (input)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        AddVehicle();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        MoveVehicle();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        RemoveVehicle();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        SearchVehicle();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        SearchSpot();
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        OptimizeBikes();
                        break;
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        OptimizeParking();
                        break;
                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        ParkingOverview();
                        break;
                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.Escape:
                        ExitQuestion();
                        break;
                    default:
                        break;
                }
            }
        }
        private string GetReg()
        {
            string regnr;
            bool validInput;
            do
            {
                validInput = true;
                regnr = "";
                Console.Write("Enter reg nr: ");
                while (regnr.Length < 1)
                {
                    regnr = Console.ReadLine().ToUpper();
                }
                if (regnr.Contains(" "))
                {
                    Console.WriteLine("Reg numbers must not contain any whitespaces.");
                    validInput = false;
                }
                else if (regnr.Contains("@"))
                {
                    Console.WriteLine("Reg numbers must not contain any '@' symbols.");
                    validInput = false;
                }
            } while (!validInput);
            return regnr;
        }
        private Vehicle.VehicleType GetVehicleType()
        {
            while (true)
            {
                Console.WriteLine("Is this a car or a motorcycle? Answer with C or M: ");
                ConsoleKey input = Console.ReadKey().Key;
                Console.WriteLine();
                if(input == ConsoleKey.C)
                {
                    return Vehicle.VehicleType.CAR;
                }
                else if(input == ConsoleKey.M)
                {
                    return Vehicle.VehicleType.MOTORCYCLE;
                }
                else
                {
                    Console.WriteLine("Only answer with c, m, C or M. No other characters allowed.");
                }
            }
        }
        private int GetSpot()
        {
            bool validInput = false;
            string input;
            int output;
            do
            {
                validInput = true;
                Console.Write("Which parking spot? ");
                input = "";
                while (input.Length < 1)
                {
                    input = Console.ReadLine();
                }
                bool validInt = int.TryParse(input, out output);
                if (!validInt)
                {
                    Console.WriteLine("Only a number please.");
                    validInput = false;
                }
                if (output < 1 || output > 100)
                {
                    Console.WriteLine("Only numbers between 1-100.");
                    validInput = false;
                }
            } while (!validInput);
            return output-1;


            

        }
        private int SearchForSpot(string regnr)
        {
            for (int i = 0; i < 200; i++)
            {
                if (pLot[i].Regnr == regnr)
                {
                    return i;
                }
            }
            return -1;
        }
        private int CalculatePrice(Vehicle vehicle)
        {
            TimeSpan span = DateTime.Now - vehicle.Arrival;
            int totalPrice;
            if(span.TotalMinutes < 5)//Första 5 minuterna gratis
            {
                totalPrice = 0;
            }
            else if(span.TotalMinutes < 125)//Lägsta avgift är för 2h, +5 för första 5 minuterna gratis
            {
                if (vehicle.Type == Vehicle.VehicleType.MOTORCYCLE)
                {
                    totalPrice = 20;
                }
                else
                {
                    totalPrice = 40;
                }
            }
            else//Beräkna antal påbörjade timmar, x20 för bil, x10 för mc
            {
                int hours;
                if (span.TotalMinutes - 5 % 60 == 0)
                {
                    hours = (int)(span.TotalMinutes - 5) / 60;
                }
                else
                {
                    hours = (int)(span.TotalMinutes - 5) / 60;
                    hours++;
                }
                if(vehicle.Type == Vehicle.VehicleType.MOTORCYCLE)
                {
                    totalPrice = hours * 10;
                }
                else
                {
                    totalPrice = hours * 20;
                }
            }
            return totalPrice;

        }
        private void PrintSearch(int index)
        {
            if (pLot[index].Type == Vehicle.VehicleType.EMPTY)
            {
                Console.WriteLine("That spot is empty.");
            }
            else if (index < 100)
            {
                int price = CalculatePrice(pLot[index]);
                TimeSpan span = DateTime.Now - pLot[index].Arrival;
                Console.WriteLine("{0} ({1}), parked at spot {2}, arrived {3}.\nParked time: {4:0,0} hours {5} minutes. Cost so far: {6:C}.", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), index + 1, pLot[index].Arrival, span.TotalHours, span.Minutes, price);
                if (pLot[index+100].Type == Vehicle.VehicleType.MOTORCYCLE)
                {
                    index += 100;
                    price = CalculatePrice(pLot[index]);
                    span = DateTime.Now - pLot[index].Arrival;
                    Console.WriteLine("{0} ({1}), is also parked at spot {2}, arrived {3}.\nParked time: {4:0,0} hours {5} minutes. Cost so far: {6:C}.", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), index - 99, pLot[index].Arrival, span.TotalHours, span.Minutes, price);
                }
            }
            else
            {
                int price = CalculatePrice(pLot[index]);
                TimeSpan span = DateTime.Now - pLot[index].Arrival;
                Console.WriteLine("{0} ({1}), parked at spot {2}, arrived {3}.\nParked time: {4:0,0} hours {5} minutes. Cost so far: {6:C}.", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), index - 99, pLot[index].Arrival, span.TotalHours, span.Minutes, price);
                index -= 100;
                price = CalculatePrice(pLot[index]);
                span = DateTime.Now - pLot[index].Arrival;
                Console.WriteLine("{0} ({1}), is also parked at spot {2}, arrived {3}.\nParked time: {4:0,0} hours {5} minutes. Cost so far: {6:C}.", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), index + 1, pLot[index].Arrival, span.TotalHours, span.Minutes, price);
            }
        }
        private void AddVehicle()
        {
            Console.Clear();
            Console.WriteLine("Add vehicle:");
            Console.WriteLine();
            bool gotSpace = false;
            DateTime tempDate = DateTime.Now;
            string tempReg = GetReg();
            Vehicle.VehicleType tempType = GetVehicleType();
            if (tempType == Vehicle.VehicleType.CAR)//Add Car
            {
                for (int i = 0; i < 100; i++)
                {
                    if (pLot[i].Type == Vehicle.VehicleType.EMPTY)
                    {
                        pLot.RemoveAt(i);
                        pLot.Insert(i, new Vehicle(tempType, tempReg, tempDate));
                        Console.WriteLine("Car {0} got assigned to spot {1}.", pLot[i].Regnr, i + 1);
                        gotSpace = true;
                        break;
                    }
                }
            }
            else//Add Bike
            {
                for (int i = 0; i < 100; i++)//Check if theres a motorcycle parked, and check +100(the place where second bike can be placed)
                {
                    if (pLot[i].Type == Vehicle.VehicleType.MOTORCYCLE)
                    {
                        if (pLot[i + 100].Type == Vehicle.VehicleType.EMPTY)//If pLot[i] contains bike and [i+100] doesn't, put a bike there (doubleparking)
                        {
                            pLot.RemoveAt(i + 100);
                            pLot.Insert(i + 100, new Vehicle(tempType, tempReg, tempDate));
                            Console.WriteLine("Motorcycle {0} got assigned to spot {1}. This spot also holds the motorcycle {2}", pLot[i + 100].Regnr, i + 1, pLot[i].Regnr);
                            gotSpace = true;
                            break;
                        }
                    }
                }
                if (!gotSpace)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (pLot[i].Type == Vehicle.VehicleType.EMPTY)
                        {
                            pLot.RemoveAt(i);
                            pLot.Insert(i, new Vehicle(tempType, tempReg, tempDate));
                            Console.WriteLine("Motorcycle {0} got assigned to spot {1}.", pLot[i].Regnr, i + 1);
                            gotSpace = true;
                            break;
                        }
                    }
                }
            }
            if (!gotSpace)
            {
                Console.WriteLine("Sorry, there is no spot for a {0} at this moment.", tempType.ToString().ToLower());
            }
            else
            {
                ReadWrite rw = new ReadWrite();
                try
                {
                    rw.WriteDatabase(pLot);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("Press enter to return to the main menu.");
            Console.Read();
        }
        private void MoveVehicle()
        {
            Console.Clear();
            Console.WriteLine("Move Vehicle:");
            Console.WriteLine();
            string regnr = GetReg();
            int indexOf = SearchForSpot(regnr);           
            if (indexOf < 0)
            {
                Console.WriteLine("Couldn't find {0} at the parking lot, sorry.", regnr);
            }
            else
            {
                int desiredSpot = GetSpot();
                //AKA if the one to move is a motorcycle, there is a motorcycle at the desired spot, but only one (aka no-one at spot desiredSpot+100)
                if (pLot[indexOf].Type == Vehicle.VehicleType.MOTORCYCLE && pLot[desiredSpot].Type == Vehicle.VehicleType.MOTORCYCLE && pLot[desiredSpot + 100].Type == Vehicle.VehicleType.EMPTY)
                {
                    Vehicle temp = pLot[desiredSpot + 100];
                    pLot[desiredSpot + 100] = pLot[indexOf];
                    pLot[indexOf] = temp;
                    if(indexOf < 100)//As long as the motorcycle that was moved is not a "secondary" motorcycle
                    {
                        if (pLot[indexOf + 100].Type == Vehicle.VehicleType.MOTORCYCLE) //If there's another motorcycle in the spot, move it to the "main" index
                        {
                            temp = pLot[indexOf + 100];
                            pLot[indexOf + 100] = pLot[indexOf];
                            pLot[indexOf] = temp;
                        }
                    }
                    Console.WriteLine("{0} moved to spot {1}, which also contains {2}.", pLot[desiredSpot + 100].Regnr, desiredSpot + 1, pLot[desiredSpot].Regnr);
                }
                //If spot is empty
                else if (pLot[desiredSpot].Type == Vehicle.VehicleType.EMPTY)
                {
                    Vehicle temp = pLot[desiredSpot];
                    pLot[desiredSpot] = pLot[indexOf];
                    pLot[indexOf] = temp;
                    if (indexOf < 100)//As long as the motorcycle that was moved is not a "secondary" motorcycle
                    {
                        if (pLot[indexOf + 100].Type == Vehicle.VehicleType.MOTORCYCLE) //If there's another motorcycle in the spot, move it to the "main" index
                        {
                            temp = pLot[indexOf + 100];
                            pLot[indexOf + 100] = pLot[indexOf];
                            pLot[indexOf] = temp;
                        }
                    }
                    Console.WriteLine("{0} ({1}) moved to spot {2}.", pLot[desiredSpot].Regnr, pLot[desiredSpot].Type.ToString().ToLower(), desiredSpot + 1);
                }
                else
                {
                    Console.WriteLine("Sorry, but there is no place for {0} at {1}.", pLot[indexOf].Regnr, desiredSpot);
                }

            }
            ReadWrite rw = new ReadWrite();
            try
            {
                rw.WriteDatabase(pLot);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press enter to return to the menu.");
            Console.ReadLine();
        }
        private void RemoveVehicle()
        {
            Console.Clear();
            Console.WriteLine("Remove a vehicle:");
            Console.WriteLine();
            string tempReg = GetReg();
            int index = SearchForSpot(tempReg);
            if(index > 0 && index < 100)
            {
                int price = CalculatePrice(pLot[index]);
                Console.WriteLine("{0} ({1}) arrived at the parking lot {2}. The total fee is {3:C}.\nThe {1} is situated at spot {4}", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), pLot[index].Arrival, price, index+1);
                if(pLot[index].Type == Vehicle.VehicleType.MOTORCYCLE && pLot[index+100].Type == Vehicle.VehicleType.MOTORCYCLE)
                {
                    pLot[index] = pLot[index + 100];
                    pLot[index + 100] = new Vehicle(Vehicle.VehicleType.EMPTY, "EMPTY", DateTime.MinValue);
                }
                else
                {
                    pLot[index] = new Vehicle(Vehicle.VehicleType.EMPTY, "EMPTY", DateTime.MinValue);
                }
            }
            else if(index >= 100)
            {
                int price = CalculatePrice(pLot[index]);
                Console.WriteLine("{0} ({1}) arrived at the parking lot {2}. The total fee is {3:C}.\nThe {1} is situated at spot {4}", pLot[index].Regnr, pLot[index].Type.ToString().ToLower(), pLot[index].Arrival, price, index - 99);
                pLot[index] = new Vehicle(Vehicle.VehicleType.EMPTY, "EMPTY", DateTime.MinValue);
            }
            else
            {
                Console.WriteLine("That vehicle ({0}) is not parked here.", tempReg);
            }
            ReadWrite rw = new ReadWrite();
            try
            {
                rw.WriteDatabase(pLot);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press Enter to return to the menu.");
            Console.ReadLine();
        }
        private void SearchVehicle()
        {
            Console.Clear();
            Console.WriteLine("Search for a vehicle:");
            Console.WriteLine();
            string regnr = GetReg();
            int indexOf = SearchForSpot(regnr);
            if (indexOf < 0)
            {
                Console.WriteLine("That vehicle ({0}) can't be found at the parking lot, sorry.", regnr);
            }
            else
            {
                PrintSearch(indexOf);
            }
            Console.WriteLine("Press Enter to return to the main menu.");
            Console.ReadLine();
        }
        private void SearchSpot()
        {
            Console.Clear();
            Console.WriteLine("Search in a spot:");
            Console.WriteLine();
            int index = GetSpot();
            PrintSearch(index);
            Console.WriteLine("Press Enter to return to the main menu.");
            Console.ReadLine();
        }
        private void OptimizeBikes()
        {
            Console.Clear();
            Console.WriteLine("Optimize motorcycle parking:");
            Console.WriteLine();
            for(int i=0; i<100; i++)
            {
                if(pLot[i].Type == Vehicle.VehicleType.MOTORCYCLE && pLot[i+100].Type == Vehicle.VehicleType.EMPTY)
                {
                    for(int j=i+1; j<100; j++)
                    {
                        if(pLot[j].Type == Vehicle.VehicleType.MOTORCYCLE && pLot[j+100].Type == Vehicle.VehicleType.EMPTY)
                        {
                            Vehicle temp = pLot[i+100];
                            pLot[i + 100] = pLot[j];
                            pLot[j] = temp;
                            Console.WriteLine("Move {0} from spot {1} to spot {2}.", pLot[i + 100].Regnr, j + 1, i + 1);
                            break;
                        }
                    }
                }
            }
            ReadWrite rw = new ReadWrite();
            try
            {
                rw.WriteDatabase(pLot);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Optimization finished. Press Enter to return to main menu.");
            Console.ReadLine();
        }
        private void OptimizeParking()
        {
            Console.Clear();
            Console.WriteLine("Optimize parking:");
            Console.WriteLine();
            for(int i=0; i<100; i++)
            {
                if(pLot[i].Type == Vehicle.VehicleType.EMPTY)
                {
                    for(int j=99; j>i; j--)
                    {
                        if(pLot[j].Type == Vehicle.VehicleType.CAR)
                        {
                            Vehicle temp = pLot[i];
                            pLot[i] = pLot[j];
                            pLot[j] = temp;
                            Console.WriteLine("Move (car) {0} from spot {1} to spot {2}.", pLot[i].Regnr, j + 1, i + 1);
                            break;
                        }
                        else if(pLot[j].Type == Vehicle.VehicleType.MOTORCYCLE)
                        {
                            if(pLot[j+100].Type == Vehicle.VehicleType.MOTORCYCLE)
                            {
                                Vehicle temp = pLot[i];
                                pLot[i] = pLot[j];
                                pLot[i + 100] = pLot[j + 100];
                                pLot[j] = temp;
                                pLot[j + 100] = temp;
                                Console.WriteLine("Move (motorcycles) {0} and {1}, from spot {2} to spot {3}.", pLot[i].Regnr, pLot[i + 100].Regnr, j + 1, i + 1);
                                break;
                            }
                            else
                            {
                                Vehicle temp = pLot[i];
                                pLot[i] = pLot[j];
                                pLot[j] = temp;
                                Console.WriteLine("Move (motorcycle) {0} from spot {1} to spot {2}.", pLot[i].Regnr, j + 1, i + 1);
                                break;
                            }
                        }
                    }
                }
            }
            ReadWrite rw = new ReadWrite();
            try
            {
                rw.WriteDatabase(pLot);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Optimization finished. Press Enter to return to main menu.");
            Console.ReadLine();
        }
        private void ParkingOverview()
        {
            Console.Clear();
            Console.WriteLine("Parking lot overview:");
            int spot = 1;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Cars, yellow");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Motorcycles, blue");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Empty, red");
            Console.ResetColor();
            Console.WriteLine("__________________________________________________________________");//Snygga till översta raden
            for (int i = 0; i < 20; i++)//Skriver ut alla parkeringsplatser, med nummer
            {
                Console.WriteLine("|            |            |            |            |            |");
                Console.WriteLine("|            |            |            |            |            |");
                Console.WriteLine("|            |            |            |            |            |");
                Console.WriteLine("|    {0:000}     |    {1:000}     |    {2:000}     |    " +
                    "{3:000}     |     {4:000}    |", spot++, spot++, spot++, spot++, spot++);
                Console.WriteLine("|____________|____________|____________|____________|____________|");
            }
            for(int i= 0; i<100; i++)
            {
                int x = ((i % 5) * 13) + 1;   //Uträkningar som ger x/y-koordinater för att skriva ut regnr
                int y = (((i / 5) + 1) * 5)+1;
                if (pLot[i].Type == Vehicle.VehicleType.CAR)
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(pLot[i].Regnr);
                    Console.ResetColor();
                }
                else if(pLot[i].Type == Vehicle.VehicleType.MOTORCYCLE)
                {
                    if(pLot[i+100].Type == Vehicle.VehicleType.MOTORCYCLE)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(pLot[i].Regnr);
                        Console.SetCursorPosition(x, y+1);
                        Console.Write(pLot[i + 100].Regnr);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(pLot[i].Regnr);
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Empty");
                    Console.ResetColor();
                }
            }
            Console.SetCursorPosition(0, 107);
            Console.WriteLine("Press Enter to return to the main menu.");
            Console.ReadLine();
        }
        private void ExitQuestion()
        {
            Console.Clear();
            Console.WriteLine("Exit");
            Console.WriteLine();
            Console.Write("Are you sure you want to exit? Y/N");
            ConsoleKey exitYN = Console.ReadKey().Key;
            if (exitYN == ConsoleKey.Y || exitYN == ConsoleKey.Escape)
            {
                System.Environment.Exit(0);
            }
        }

    }
}
