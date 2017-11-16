using System;

namespace ObjectPrinting.Tests
{
	public class Person
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public double Height { get; set; }
		public Dog Dog{
			get;
			set;
		}
		public int Age { get; set; }
	}

	public class Dog
	{
		public string Name { get; set; }
	}
}