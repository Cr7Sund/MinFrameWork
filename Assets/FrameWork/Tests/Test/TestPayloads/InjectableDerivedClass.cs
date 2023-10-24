using System;

namespace Cr7Sund.Framework.Tests
{
	public class InjectableDerivedClass : InjectableSuperClass
	{
		[Inject]
		public ClassToBeInjected injected;

		public InjectableDerivedClass()
		{
		}

		[PostConstruct]
		public void postConstruct1()
		{
			Console.Write("Calling post construct 1\n");
		}



		public void notAPostConstruct()
		{
			Console.Write("notAPostConstruct :: SHOULD NOT CALL THIS!");
		}
	}

	public class InjectableDerivedClassOne : InjectableSuperClass
	{
		[Inject]
		public ClassToBeInjected injected;

	}
	public class InjectableDerivedClassTwo : InjectableSuperClass
	{
		[Inject]
		public ClassToBeInjected injected;
	}

	public class InjectableDerivedClassThree : InjectableSuperClass
	{
		[Inject]
		public ClassToBeInjected injected;
	}
}

