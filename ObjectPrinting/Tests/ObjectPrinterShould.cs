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
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = {defaultPerson.Dog.Name}" + Environment.NewLine +
$"	Age = {defaultPerson.Age}" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine
				);
		}

		[Test]
		public void AppliesCustomSerializatorCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing<int>().Using(x => "kek")
				.Printing<Dog>().Using(x => "Bork")
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name}" + Environment.NewLine +
$"	Height = {defaultPerson.Height}" + Environment.NewLine +
$"	Dog = Bork" + Environment.NewLine +
$"	Age = kek" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine
				);
		}
		
		[Test]
		public void AppliesCultureCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing<double>().Using(new CultureInfo("ru-RU"))
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name}" + Environment.NewLine +
$"	Height = {defaultPerson.Height.ToString(new CultureInfo("ru-RU"))}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = {defaultPerson.Dog.Name}" + Environment.NewLine +
$"	Age = {defaultPerson.Age}" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine
			);
		}
		
		[Test]
		public void CutsStrings()
		{
			ObjectPrinter.For<Person>().Printing<string>()
				.WithMaxLength(3)
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name.Substring(0, 3)}" + Environment.NewLine +
$"	Height = {defaultPerson.Height}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = {defaultPerson.Dog.Name.Substring(0, 3)}" + Environment.NewLine +
$"	Age = {defaultPerson.Age}" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine
			);
		}
		[Test]
		public void AppliesCustomPropertySerializatorCorrectly()
		{
			ObjectPrinter.For<Person>()
				.Printing(p => p.Dog.Name).Using(x => "k-9")
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name}" + Environment.NewLine +
$"	Height = {defaultPerson.Height}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = k-9" + Environment.NewLine +
$"	Age = {defaultPerson.Age}" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine
			);
		}
		[Test]
		public void ExcludesPropertyCorrectly()
		{
			ObjectPrinter.For<Person>()
				.ExcludingProperty(p => p.Dog)
				.PrintToString(defaultPerson).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {defaultPerson.Id}" + Environment.NewLine +
$"	Name = {defaultPerson.Name}" + Environment.NewLine +
$"	Height = {defaultPerson.Height}" + Environment.NewLine +
$"	Age = {defaultPerson.Age}" + Environment.NewLine +
$"	Brotha = null" + Environment.NewLine 
			);
		}
		[Test]
		public void ExcludesPropertyCorrectlyOnNestedClass()
		{
			var person = new Person { Name = "Alex", Height = 20.2, Age = 19 , Dog = new Dog { Name = "Woofer" },
				Brotha = new Person { Name = "Jeff", Height = 200.0, Age = 12, Dog = new Dog { Name = "Barkinson"}}};
			ObjectPrinter.For<Person>()
				.ExcludingProperty(p => p.Brotha.Dog.Name)
				.PrintToString(person).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {person.Id}" + Environment.NewLine +
$"	Name = {person.Name}" + Environment.NewLine +
$"	Height = {person.Height}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = {person.Dog.Name}" + Environment.NewLine +
$"	Age = {person.Age}" + Environment.NewLine +
$"	Brotha = Person" + Environment.NewLine +
$"		Id = {person.Brotha.Id}" + Environment.NewLine +
$"		Name = {person.Brotha.Name}" + Environment.NewLine +
$"		Height = {person.Brotha.Height}" + Environment.NewLine +
$"		Dog = Dog" + Environment.NewLine +
$"		Age = {person.Brotha.Age}" + Environment.NewLine +
$"		Brotha = null" + Environment.NewLine
			);
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
			.PrintToString(person).Should().Be(
$"Person" + Environment.NewLine +
$"	Id = {person.Id}" + Environment.NewLine +
$"	Name = {person.Name}" + Environment.NewLine +
$"	Height = {person.Height}" + Environment.NewLine +
$"	Dog = Dog" + Environment.NewLine +
$"		Name = {person.Dog.Name}" + Environment.NewLine +
$"	Age = {person.Age}" + Environment.NewLine +
$"	Brotha = Person" + Environment.NewLine +
$"		Id = {person.Brotha.Id}" + Environment.NewLine +
$"		Name = Michael"+ Environment.NewLine +
$"		Height = {person.Brotha.Height}" + Environment.NewLine +
$"		Dog = Dog" + Environment.NewLine +
$"			Name = Dogster" + Environment.NewLine +
$"		Age = {person.Brotha.Age}" + Environment.NewLine +
$"		Brotha = null" + Environment.NewLine
			);
		}
	}
}