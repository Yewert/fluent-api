using System;
using System.Globalization;
using ObjectPrinting.Tests;

namespace ObjectPrinting
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "woof"}};

            var printer = ObjectPrinter.For<Person>()
                .Printing<Dog>().Using(x => "kek")
                .Printing<double>().Using(new CultureInfo("ru-RU"))
                .ExcludingType<int>();
            
            Console.Write(printer.PrintToString(person));
        }
    }
}