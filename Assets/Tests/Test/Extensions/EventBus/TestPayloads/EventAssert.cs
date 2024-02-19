using System;
using System.Collections.Generic;
using Cr7Sund.PackageTest.EventBus.Api;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = NUnit.Framework.Is;

namespace Cr7Sund.PackageTest.EventBus
{
	public static class EventAssert
	{
		public static void AssertDoesNotAllocate(Action action)
		{
			Assert.That(() => action(), Is.Not.AllocatingGCMemory());
		}

		public static void AssertListenersInvokedInOrder<TEvent>(this TestEventBus bus, TEvent eventData,
			params TestListener[] listeners) where TEvent : IEventData, new()
		{
			AssertListenersInvokedInOrder(bus, eventData, listeners, listeners);
		}

		public static void AssertListenersInvokedInOrder<TEvent>(this TestEventBus bus, TEvent eventData,
			TestListener<TEvent>[] listeners, TestListener<TEvent>[] expectedOrder) where TEvent : IEventData, new()
		{
			AssertListenersInvokedInOrder(bus, eventData, (TestListener[])listeners, expectedOrder);
		}

		public static void AssertListenersInvokedInOrder<TEvent>(this TestEventBus bus, TEvent eventData,
			TestListener[] listeners, TestListener[] expectedOrder) where TEvent : IEventData, new()
		{
			var eventReceivedList = new List<TestListener>(listeners.Length);

			foreach (var listener in listeners)
			{
				listener.EventReceivedEvent += OnListenerReceivedEvent;
			}

			bus.Raise(eventData);

			Assert.AreEqual(expectedOrder.Length, eventReceivedList.Count);

			CollectionAssert.AreEqual(expectedOrder, eventReceivedList, "Listeners were not invoked in the expected order.");

			void OnListenerReceivedEvent(TestListener listener)
			{
				listener.EventReceivedEvent -= OnListenerReceivedEvent;

				eventReceivedList.Add(listener);
			}
		}
	}
}