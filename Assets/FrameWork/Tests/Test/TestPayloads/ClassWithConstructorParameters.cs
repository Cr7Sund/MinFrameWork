using System;

namespace Cr7Sund.Framework.Tests
{
	public class ClassWithConstructorParameters : ISimpleInterface
	{
		private int _intValue;
		private string _stringValue;

		//Two constructors. One is tagged to be the constructor used during injection
		public ClassWithConstructorParameters()
		{
			this._intValue = 42;
			this._stringValue = "Liberator";
		}

		public ClassWithConstructorParameters(int intValue, string stringValue)
		{
			this._intValue = intValue;
			this._stringValue = stringValue;
		}

		public void DeConstruct()
		{
			this._intValue = 0;
		}

		public int intValue
		{
			get
			{
				return _intValue;
			}
			set
			{
				_intValue = value;
			}
		}

		public string stringValue
		{
			get
			{
				return _stringValue;
			}
			set
			{
				_stringValue = value;
			}
		}
	}
}

