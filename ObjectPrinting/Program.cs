﻿using System;
using ObjectPrinting.Tests;

namespace ObjectPrinting
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
                Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};

            var printer = ObjectPrinter.For<Person>().ExcludingProperty(p => p.Brotha.Dog.Name);
            
            Console.Write(printer.PrintToString(person));
            
            Console.WriteLine("=== same using extension method ===");
            
            Console.Write(person.PrintToString(pr => pr.ExcludingProperty(p => p.Brotha.Dog.Name)));
        }
    }
}