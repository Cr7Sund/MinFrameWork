/**
 * @interface Cr7Sund.Framework.Api.IReflectedClass
 * 
 * Interface for representation of a class.
 * 
 * A reflection represents the already-reflected class, complete with the preferred
 * constructor, the constructor parameters, post-constructor(s) and settable
 * values.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cr7Sund.Framework.Api
{
	public interface IReflectedClass
	{
		/// Get/set the preferred constructor
		ConstructorInfo Constructor { get; set; }

		/// Get/set the preferred constructor's parameters length
		int ConstructorParameterCount { get; set; }

		/// Get/set any PostConstructors. This includes inherited PostConstructors.
		MethodInfo PostConstructor { get; set; }

		/// Get/set the list of field injections. This includes inherited fields.
		Tuple<Type,object, FieldInfo>[] Fields { get; set; }

	}
}

