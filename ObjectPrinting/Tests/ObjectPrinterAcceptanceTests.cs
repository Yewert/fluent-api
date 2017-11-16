using System;
using System.Globalization;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
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
				//NOT implemented
			
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				//Designed?
				//NOT implemented
			
				//6. Исключить из сериализации конкретного свойства
				//Designed
				//NOT implemented
            
            string s1 = printer.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию	
			//NOT designed
			//NOT implemented
			
			//8. ...с конфигурированием
			//NOT designed
			//NOT implemented
		}
	}
}