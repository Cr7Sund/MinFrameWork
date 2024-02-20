using System;
using System.Collections.Generic;

namespace Cr7Sund.FrameWork.Util
{
	/// <summary>
	/// Container for extension functions for the System.Collections.Generic.Dictionary{T}
	/// </summary>
	/// <license>MIT</license>
	public static class DictionaryExt
	{
		/// <summary>
		/// only return true when the dictionary is not null and contains key
		/// </summary>
		/// <param name="map">List to insert into</param>
		/// <param name="key">Value to insert</param>
		/// <typeparam name="T">Type of element to insert and type of elements in the list</typeparam>
		public static bool ContainAndNotNull<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
			where TKey : IComparable<TKey>
		{
			return map != null && map.ContainsKey(key);
		}

	}
}