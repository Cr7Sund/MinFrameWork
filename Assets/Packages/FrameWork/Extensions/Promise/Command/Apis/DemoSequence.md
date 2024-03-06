example1:

var promise = new Promise();
var comnand = commandBinder.Bind(SomeEnum.ONE).
				To<CommandWithInjectionTwo>().
				To<CommandWithExecute>().
				To<CommandWithInjectionTwo>()
						💩.Catch(e=>{}) 🎃

promise.Run(passData);

signal.Notify(SomeEnum.ONE, passData);

example2:
var promise = new Promise();
promise.Then(CommandWithInjectionTwo);
promise.Then(CommandWithExecute);
promise.ThenAll(CommandWithExecute);
promise.ThenRace(CommandWithExecute);

public void To()
{
	promise.Then(CommandWithInjectionTwo.Resolve, CommandWithInjectionTwo.Reject);
}

public class CommandWithInjectionTwo
{

	[inject] private ITestModel testModel;

	public void Resolve() where T : SequenceData
	{

	}

	public IPromise Resolve(T data) where T : SequenceData
	{
		var valuePromise = new Promise> ();
		return valuePromise;
	}

	public IPromise<T> Resolve(T data) where T : SequenceData
	{
		var valuePromise = new Promise<int>();
		return valuePromise;
	}

	public void Reject()
	{

	}

	public void Progress()
	{

	}
}