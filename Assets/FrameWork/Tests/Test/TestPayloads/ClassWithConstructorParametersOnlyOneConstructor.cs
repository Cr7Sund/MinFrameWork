using System;

namespace Cr7Sund.Framework.Tests
{
	public class ClassWithConstructorParametersOnlyOneConstructor
	{
		private string _stringVal;
		public string stringVal
		{
			get
			{
				return _stringVal;
			}

		}

		public ClassWithConstructorParametersOnlyOneConstructor()
		{
			_stringVal = "defaultValue";
		}

		public ClassWithConstructorParametersOnlyOneConstructor(string value)
		{
			_stringVal = value;

		}
	}
}

