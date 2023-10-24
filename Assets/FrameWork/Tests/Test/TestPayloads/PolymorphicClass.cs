using System;

namespace Cr7Sund.Framework.Tests
{
	public class PolymorphicClass : ISimpleInterface, IAnotherSimpleInterface
	{
		public PolymorphicClass ()
		{
		}

		#region ISimpleInterface implementation

		public int intValue { get; set;}

		#endregion

		#region IAnotherSimpleInterface implementation

		public string stringValue { get; set;}

		#endregion
	}
}

