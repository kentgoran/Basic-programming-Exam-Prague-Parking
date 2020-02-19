using System;
using System.Text;
using System.Globalization;

namespace PragueParking_2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            CultureInfo.CurrentCulture = new CultureInfo("cs-CZ");//För att snygga till det med tjeckisk valuta
            MenuMethods menu = new MenuMethods();
            menu.MainMenu();
        }
    }
}
