using System;
using UnityEngine;

namespace Cr7Sund
{
    [Serializable]
    public class StringStringDictionary : SerializableDictionary<string, string> { }

    [Serializable]
    public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> { }

    [Serializable]
    public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> { }

    [Serializable]
    public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> { }


}