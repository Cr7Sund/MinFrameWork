using UnityEngine;

namespace Cr7Sund.Editor
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class HexDrawerAttribute : PropertyAttribute
    {
        public HexDrawerAttribute()
        {
        }

    }

}