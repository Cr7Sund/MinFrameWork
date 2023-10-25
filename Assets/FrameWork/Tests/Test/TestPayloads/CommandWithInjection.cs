using System;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Tests
{
	public class CommandWithInjectionTwo : Command
	{
		[Inject]
		public ISimpleInterface injected;

		override public void Execute()
		{
			injected.intValue *= 2;
		}
	}

	public class CommandWithInjectionTen : Command
	{
		[Inject]
		public ISimpleInterface injected;

		override public void Execute()
		{
			injected.intValue *= 10;
		}
	}
}

