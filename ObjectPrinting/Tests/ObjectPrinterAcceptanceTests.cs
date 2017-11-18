﻿using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		private Person defaultPerson;
		[SetUp]
		public void SetUp()
		{
			defaultPerson = new Person {Name = "Alex", Age = 19, Height = 100.2, Dog = new Dog { Name = "Woofer" }};
		}
		
		
		[Test]
		public void Demo()
		{
			var person = new Person { Name = "Alex", Age = 19 };

			var printer = ObjectPrinter.For<Person>();
				//1. Исключить из сериализации свойства определенного типа
				//Designed
				//Implemented

				//2. Указать альтернативный способ сериализации для определенного типа
				//Designed
				//Implemented
			
				//3. Для числовых типов указать культуру
				//Designed
				//Implemented
			
				//4. Настроить сериализацию конкретного свойства
				//Designed
				//Implemented
			
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				//Designed
				//Implemented
			
				//6. Исключить из сериализации конкретного свойства
				//Designed
				//Implemented
            
            string s1 = printer.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию	
			//NOT designed
			//NOT implemented
			
			//8. ...с конфигурированием
			//NOT designed
			//NOT implemented
		}

		[Test]
		public void ExcludesTypeCorrectly()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.ExcludingType<double>();
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Dog = Dog
		Name = Woofer
	Age = 19
	Brotha = null
"
				);
		}

		[Test]
		public void AppliesCustomSerializatorCorrectly()
		{
			var printer = ObjectPrinter.For<Person>();
			printer
				.Printing<int>().Using(x => "kek")
				.Printing<Dog>().Using(x => "Bork");
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 100.2
	Dog = Bork
	Age = kek
	Brotha = null
"
				);
		}
		
		[Test]
		public void AppliesCultureCorrectly()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.Printing<double>().Using(new CultureInfo("ru-RU"));
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 100,2
	Dog = Dog
		Name = Woofer
	Age = 19
	Brotha = null
"
			);
		}
		
		[Test]
		public void CutsStrings()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.Printing<string>().WithMaxLength(3);
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Ale
	Height = 100.2
	Dog = Dog
		Name = Woo
	Age = 19
	Brotha = null
"
			);
		}
		[Test]
		public void AppliesCustomPropertySerializatorCorrectly()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.Printing(p => p.Dog.Name).Using(x => "k-9");
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 100.2
	Dog = Dog
		Name = k-9
	Age = 19
	Brotha = null
"
			);
		}
		[Test]
		public void ExcludesPropertyCorrectly()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.ExcludingProperty(p => p.Dog);
			printer.PrintToString(defaultPerson).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 100.2
	Age = 19
	Brotha = null
"
			);
		}
		[Test]
		public void ExcludesPropertyCorrectlyOnNestedClass()
		{
			var printer = ObjectPrinter.For<Person>();
			printer.ExcludingProperty(p => p.Brotha.Dog.Name);
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			printer.PrintToString(person).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 20.2
	Dog = Dog
		Name = Woofer
	Age = 19
	Brotha = Person
		Id = 00000000-0000-0000-0000-000000000000
		Name = Jeff
		Height = 200
		Dog = Dog
		Age = 12
		Brotha = null
"
			);
		}
		
		[Test]
		public void PrintsNestedPropertiesWithCustomSerializator()
		{
			var printer = ObjectPrinter.For<Person>();
			printer
				.Printing(pers => pers.Brotha.Dog.Name)
					.Using(x => "Dogster")
				.Printing(p => p.Brotha.Name)
					.Using(x => "Michael");
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			printer.PrintToString(person).Should().Be(
@"Person
	Id = 00000000-0000-0000-0000-000000000000
	Name = Alex
	Height = 20.2
	Dog = Dog
		Name = Woofer
	Age = 19
	Brotha = Person
		Id = 00000000-0000-0000-0000-000000000000
		Name = Michael
		Height = 200
		Dog = Dog
			Name = Dogster
		Age = 12
		Brotha = null
"
			);
		}
	}
}