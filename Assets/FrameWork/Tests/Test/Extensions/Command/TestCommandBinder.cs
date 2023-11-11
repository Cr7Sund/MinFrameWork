using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
	public class TestCommandBinder
	{
		IInjectionBinder injectionBinder;
		ICommandBinder commandBinder;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInjectionBinder>().Bind<IInstanceProvider>().ToValue(injectionBinder);
			injectionBinder.Bind<ICommandBinder>().To<CommandBinder>().AsSingleton();
			commandBinder = injectionBinder.GetInstance<ICommandBinder>();

			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().AsSingleton();
			injectionBinder.GetInstance<ISimpleInterface>().intValue = 3;
		}

		[Test]
		public void TestExecuteInjectionCommand()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjectionTwo>();
			commandBinder.ReactTo(SomeEnum.ONE);

			//The command should set the value to 100
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>();
			Assert.AreEqual(6, instance.intValue);
		}

		[Test]
		public void TestMultipleCommands()
		{
			//Bind the trigger to the command
			// we also support same action execute twice
			commandBinder.Bind(SomeEnum.ONE)
				.To<CommandWithInjectionTwo>().To<CommandWithExecute>()
				.To<CommandWithInjectionTwo>()
				.To<CommandWithoutExecute>()
				.To<CommandWithInjectionTwo>();

			TestDelegate testDelegate = delegate
			{
				commandBinder.ReactTo(SomeEnum.ONE);
			};

			//That the exception is thrown demonstrates that the last command ran
			CommandException ex = Assert.Throws<CommandException>(testDelegate);
			Assert.AreEqual(ex.Type, CommandExceptionType.EXECUTE_OVERRIDE);

			//That the value is 6 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual(12, instance.intValue);
		}

		[Test]
		public void TestNotOnce()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjectionTwo>();

			ICommandBinding binding = commandBinder.GetBinding(SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull(binding);

			commandBinder.ReactTo(SomeEnum.ONE);

			ICommandBinding binding2 = commandBinder.GetBinding(SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull(binding2);
		}

		[Test]
		public void TestOnce()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjectionTwo>().Once();

			ICommandBinding binding = commandBinder.GetBinding(SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull(binding);

			commandBinder.ReactTo(SomeEnum.ONE);

			ICommandBinding binding2 = commandBinder.GetBinding(SomeEnum.ONE) as ICommandBinding;
			Assert.IsNull(binding2);
		}

		[Test]
		public void TestSequence()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE)
			.To<CommandWithExecute>()
			.To<CommandWithInjectionTwo>()
			.To<CommandWithoutExecute>().InSequence();

			TestDelegate testDelegate = delegate
			{
				commandBinder.ReactTo(SomeEnum.ONE);
			};

			//That the exception is thrown demonstrates that the last command ran
			CommandException ex = Assert.Throws<CommandException>(testDelegate);
			Assert.AreEqual(ex.Type, CommandExceptionType.EXECUTE_OVERRIDE);

			//That the value is 6 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual(6, instance.intValue);
		}

		[Test]
		public void TestInterruptedSequence()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE)
			.To<CommandWithInjectionTwo>()
			.To<CancelCommand>()
			.To<CommandWithoutExecute>().InSequence();

			TestDelegate testDelegate = delegate
			{
				commandBinder.ReactTo(SomeEnum.ONE);
			};

			//That the exception is not thrown demonstrates that the last command was interrupted
			Assert.DoesNotThrow(testDelegate);

			//That the value is 6 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual(6, instance.intValue);
		}

		[Test]
		public void TestInterruptedGroupSequence()
		{
			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE)
			.To<CommandWithInjectionTwo>()
			.To<CancelCommand>()
			.To<CommandWithInjectionTwo>()
			.To<CommandWithoutExecute>().InSequence();

			commandBinder.Bind(SomeEnum.TWO)
				.To<CommandWithInjectionTen>()
				.To<CommandWithInjectionTwo>()
				.To<CancelCommand>()
				.To<CommandWithInjectionTwo>()
			.InSequence();

			TestDelegate testDelegate = delegate
			{
				commandBinder.ReactTo(SomeEnum.ONE);
				commandBinder.ReactTo(SomeEnum.TWO);
			};

			//That the exception is not thrown demonstrates that the last command was interrupted
			Assert.DoesNotThrow(testDelegate);

			//That the value is 6 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual(6 * 2 * 10, instance.intValue);
		}

		//TODO: figure out how to do async tests
		/*
		[Test]
		public async void TestAsyncCommand()
		{
			injectionBinder.Bind<Timer>().To<Timer> ();
			commandBinder.Bind (SomeEnum.ONE).To<AsynchCommand> ();
			Task<bool> answer = commandBinder.ReactTo (SomeEnum.ONE);

			//Assert.Throws<Exception> ( await );
		}
		*/
	}
}

