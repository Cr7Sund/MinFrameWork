using UnityEditor;

namespace Cr7Sund.Editor
{
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    [CustomPropertyDrawer(typeof(ObjectColorDictionary))]
    [CustomPropertyDrawer(typeof(StringColorArrayDictionary))]

    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    [CustomPropertyDrawer(typeof(ColorArrayStorage))]
    public class AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }
}