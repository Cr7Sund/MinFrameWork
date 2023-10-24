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
		/// <summary>  Get/set the preferred constructor </summary> 
		ConstructorInfo Constructor { get; set; }

		/// <summary>  Get/set the preferred constructor's parameters length </summary> 
		int ConstructorParameterCount { get; set; }

		/// <summary>  Get/set any PostConstructors. This includes inherited PostConstructors. </summary> 
		MethodInfo PostConstructor { get; set; }

		/// <summary>  Get/set the list of field injections. This includes inherited fields. </summary> 
		Tuple<Type,object, FieldInfo>[] Fields { get; set; }

	}
}

