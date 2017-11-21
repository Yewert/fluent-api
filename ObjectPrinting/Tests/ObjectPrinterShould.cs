﻿using System;
 using System.Globalization;
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
		public void ExcludesTypeCorrectly()
		{
			ObjectPrinter.For<Person>()
				.ExcludingType<double>()
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name}",
						$"	Dog = Dog",
						$"		Name = {defaultPerson.Dog.Name}",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}

		[Test]
		public void AppliesCustomSerializatorCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing<int>()
					.Using(x => "kek")
				.Printing<Dog>()
					.Using(x => "Bork")
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person" + Environment.NewLine +
						$"	Id = {defaultPerson.Id}" + Environment.NewLine +
						$"	Name = {defaultPerson.Name}" + Environment.NewLine +
						$"	Height = {defaultPerson.Height}" + Environment.NewLine +
						$"	Dog = Bork" + Environment.NewLine +
						$"	Age = kek" + Environment.NewLine +
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		
		[Test]
		public void AppliesCultureCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing<double>().Using(new CultureInfo("ru-RU"))
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name}",
						$"	Height = {defaultPerson.Height.ToString(new CultureInfo("ru-RU"))}",
						$"	Dog = Dog",
						$"		Name = {defaultPerson.Dog.Name}",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		
		[Test]
		public void CutsStrings()
		{
			ObjectPrinter.For<Person>().Printing<string>()
				.WithMaxLength(3)
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name.Substring(0, 3)}",
						$"	Height = {defaultPerson.Height}",
						$"	Dog = Dog",
						$"		Name = {defaultPerson.Dog.Name.Substring(0, 3)}",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		[Test]
		public void AppliesCustomPropertySerializatorCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing(p => p.Dog.Name).Using(x => "k-9")
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name}",
						$"	Height = {defaultPerson.Height}",
						$"	Dog = Dog",
						$"		Name = k-9",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		[Test]
		public void ExcludesPropertyCorrectly()
		{
			ObjectPrinter.For<Person>()
				.ExcludingProperty(p => p.Dog)
				.PrintToString(defaultPerson)
				.Should().Be(
					string.Join(Environment.NewLine,
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name}",
						$"	Height = {defaultPerson.Height}",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		[Test]
		public void ExcludesPropertyCorrectlyOnNestedClass()
		{
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			ObjectPrinter.For<Person>()
				.ExcludingProperty(p => p.Brotha.Dog.Name)
				.PrintToString(person)
				.Should().Be(
					string.Join(Environment.NewLine,
						$"Person",
						$"	Id = {person.Id}",
						$"	Name = {person.Name}",
						$"	Height = {person.Height}",
						$"	Dog = Dog",
						$"		Name = {person.Dog.Name}",
						$"	Age = {person.Age}",
						$"	Brotha = Person",
						$"		Id = {person.Brotha.Id}",
						$"		Name = {person.Brotha.Name}",
						$"		Height = {person.Brotha.Height}",
						$"		Dog = Dog",
						$"		Age = {person.Brotha.Age}",
						$"		Brotha = null")
					+ Environment.NewLine);
		}
		
		[Test]
		public void PrintsNestedPropertiesWithCustomSerializator()
		{
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			ObjectPrinter.For<Person>()
				.Printing(pers => pers.Brotha.Dog.Name)
					.Using(x => "Dogster")
				.Printing(p => p.Brotha.Name)
					.Using(x => "Michael")
				.PrintToString(person)
				.Should().Be(
					string.Join(Environment.NewLine,
						$"Person",
						$"	Id = {person.Id}",
						$"	Name = {person.Name}",
						$"	Height = {person.Height}",
						$"	Dog = Dog",
						$"		Name = {person.Dog.Name}",
						$"	Age = {person.Age}",
						$"	Brotha = Person",
						$"		Id = {person.Brotha.Id}",
						$"		Name = Michael",
						$"		Height = {person.Brotha.Height}",
						$"		Dog = Dog",
						$"			Name = Dogster",
						$"		Age = {person.Brotha.Age}",
						$"		Brotha = null") 
					+ Environment.NewLine);
		}
		
		[Test]
		public void PrintsCorrectlyUsingDefaultSerialiatorFromExtensionMethod()
		{
			defaultPerson.PrintToString()
				.Should().Be(
					string.Join(Environment.NewLine,
						$"Person",
						$"	Id = {defaultPerson.Id}",
						$"	Name = {defaultPerson.Name}",
						$"	Height = {defaultPerson.Height}",
						$"	Dog = Dog",
						$"		Name = {defaultPerson.Dog.Name}",
						$"	Age = {defaultPerson.Age}",
						$"	Brotha = null")
					+ Environment.NewLine);
		}
		
		[Test]
		public void PrintsNestedPropertiesWithCustomSerializatorUsingExtensionMethod()
		{
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			person.PrintToString(pr => pr
					.Printing(pers => pers.Brotha.Dog.Name)
						.Using(x => "Dogster")
					.Printing(p => p.Brotha.Name)
						.Using(x => "Michael"))
				.Should().Be(
					string.Join(Environment.NewLine, 
						$"Person",
						$"	Id = {person.Id}",
						$"	Name = {person.Name}",
						$"	Height = {person.Height}",
						$"	Dog = Dog",
						$"		Name = {person.Dog.Name}",
						$"	Age = {person.Age}",
						$"	Brotha = Person",
						$"		Id = {person.Brotha.Id}",
						$"		Name = Michael",
						$"		Height = {person.Brotha.Height}",
						$"		Dog = Dog",
						$"			Name = Dogster",
						$"		Age = {person.Brotha.Age}",
						$"		Brotha = null")
					+ Environment.NewLine);
		}
	}
}