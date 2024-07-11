using UnityEditor;
using UnityEditor.UIElements;

namespace Cr7Sund.Editor
{
    public interface IResetSettings
    {
        string pathPartialToCategory { get; }
        SettingsScope scope { get; }
        string[] tags { get; }
        UnityEngine.Object instance{get;}

        void CopyAllFromBlueprint(SerializedObject serializedObject);
        void CopyFromBlueprint(SerializedProperty property);
        void NotifyValueChanged(SerializedPropertyChangeEvent evt);
        void Save();
    }

}