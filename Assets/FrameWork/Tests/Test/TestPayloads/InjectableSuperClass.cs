using System;

namespace Cr7Sund.Framework.Tests
{
	public class InjectableSuperClass
	{
		public float floatValue{get;set;}

		[Inject]
		public int intValue;

		public InjectableSuperClass ()
		{
		}
	}
}

