using Cr7Sund.Package.EventBus.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Api;

namespace Cr7Sund.PackageTest.EventBus
{
	public class TestEventBus : GenericEventBus<IEventData>
	{
		[Inject] private IPoolBinder _poolBinder;

		protected override IPoolBinder poolBinder => _poolBinder;
	}

}