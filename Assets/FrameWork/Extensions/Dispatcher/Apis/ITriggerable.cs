/**
 * @interface Cr7Sund.Framework.Api.ITrigerable
 * 
 * Interface for declaring a class capable of being Trigered by a provided key and/or name.
 * 
 * Objects implementing ITrigerable can register with objects implementing
 * ITrigerProvider. The contract specifies that TrigerProvider will
 * pass events on to the Trigerable class. This allows notifications,
 * such as IEvents to pass through the event bus and Trigger other binders.
 * 
 * @see Cr7Sund.Framework.Api.ITrigerProvider
 */


namespace Cr7Sund.Framework.Api
{
	public interface ITriggerable
	{
		/// Cause this ITrigerable to access any provided Key in its Binder by the provided generic and data.
		/// <returns>false if the originator should abort dispatch</returns>
		bool Trigger<T>(object data);

		/// Cause this ITrigerable to access any provided Key in its Binder by the provided key and data.
		/// <returns>false if the originator should abort dispatch</returns>
		bool Trigger(object key, object data);
	}
}

