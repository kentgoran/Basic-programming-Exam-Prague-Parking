using System;
using System.Collections.Generic;
using System.IO;

namespace PragueParking_2._0
{
    class ReadWrite
    {
        public List<Vehicle> ReadDatabase()
        {
            if (!File.Exists("database.txt"))
            {
                throw new FileNotFoundException("The file 'database.txt' could not be found. Couldn't read the file.");
            }
            StreamReader sr = new StreamReader("database.txt");
            List<Vehicle> vehicles = new List<Vehicle>();
            string temp = "";
            using (sr)
            {
                temp = sr.ReadLine();
                do
                {
                    vehicles.Add(FormatToVehicle(temp));
                    temp = sr.ReadLine();

                } while (temp != null);
            }
            return vehicles;
        }
        public void WriteDatabase(List<Vehicle> pLot)
        {
            if (!File.Exists("database.txt"))
            {
                Console.WriteLine("The file 'database.txt' does not exist. Creating a new instance of it now");
            }
            StreamWriter sw = new StreamWriter("database.txt");
            using (sw)
            {
                foreach(Vehicle vehicle in pLot)
                {
                    sw.WriteLine(vehicle.ToString());
                }
                sw.Flush();
            }
        }
        private Vehicle FormatToVehicle(string input)
        {
            string[] tempArr = input.Split('@');
            DateTime tempDate = DateTime.Parse(tempArr[0]);
            Vehicle.VehicleType type;
            if(tempArr[1] == "CAR")
            {
                type = Vehicle.VehicleType.CAR;
            }
            else if(tempArr[1] == "MOTORCYCLE")
            {
                type = Vehicle.VehicleType.MOTORCYCLE;
            }
            else
            {
                type = Vehicle.VehicleType.EMPTY;
            }
            Vehicle tempVehicle = new Vehicle(type, tempArr[2], tempDate);
            return tempVehicle;
            
        }
    }
}
