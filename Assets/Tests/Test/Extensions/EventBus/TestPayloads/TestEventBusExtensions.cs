using Cr7Sund.EventBus.Api;
using Cr7Sund.EventBus.Impl;

namespace Cr7Sund.EventBus.Tests
{
	public static class TestEventBusExtensions
	{
		public static TestListener<TEvent> TestListener<TEvent>(this GenericEventBus<IEventData> bus) where TEvent : IEventData, new()
		{
			return new TestListener<TEvent>(bus);
		}

		public static TestListener<TEvent> TestListen<TEvent>(this GenericEventBus<IEventData> bus, float priority = 0,
			TestListener<TEvent>.EventReceivedHandler callback = null) where TEvent : IEventData, new()
		{
			var listener = bus.TestListener<TEvent>();
			listener.Subscribe(priority, callback);

			return listener;
		}

		public static TestListener<TEvent> TestListen<TEvent>(this GenericEventBus<IEventData> bus, System.Action callback,
			float priority = 0) where TEvent : IEventData, new()
		{
			var listener = bus.TestListener<TEvent>();
			listener.Subscribe(callback, priority);

			return listener;
		}
	}
}