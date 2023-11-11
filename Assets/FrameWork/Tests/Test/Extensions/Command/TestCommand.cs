
using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
	public class TestCommand
	{

		[Test]
		public void TestMissingExecute()
		{
			ICommand command = new CommandWithoutExecute();
			TestDelegate testDelegate = delegate ()
			{
				command.Run();
			};
			CommandException ex = Assert.Throws<CommandException>(testDelegate);
			Assert.That(ex.Type == CommandExceptionType.EXECUTE_OVERRIDE);
		}

		[Test]
		public void TestSuccessfulExecute()
		{
			ICommand command = new CommandWithExecute();
			TestDelegate testDelegate = delegate ()
			{
				command.Run();
			};
			Assert.DoesNotThrow(testDelegate);
		}

		[Test]
		public void TestRetainRelease()
		{
			ICommand command = new CommandWithExecute();
			Assert.IsFalse(command.IsRetain);
			command.Retain();
			Assert.IsTrue(command.IsRetain);
			command.Release();
			Assert.IsFalse(command.IsRetain);
		}
	}
}
