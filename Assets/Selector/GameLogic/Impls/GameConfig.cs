using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
namespace Cr7Sund.GameLogic
{
    [CreateAssetMenu(menuName = "Cr7Sund/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        #if UNITY_EDITOR
        public UnityEditorInternal.AssemblyDefinitionAsset GameAssembly;
#endif

        public Type GameLogicType;
        [FormerlySerializedAs("GameLogicName")]
        [FormerlySerializedAs("TypeInfo")]
        public string GameLogicTypeFullName;
        public string AssemblyInfo;


        public GameLogic GetLogic()
        {
            if (!string.IsNullOrEmpty(AssemblyInfo) &&
                !string.IsNullOrEmpty(GameLogicTypeFullName))
            {
                var assembly = Assembly.Load(AssemblyInfo);
                var type = assembly.GetType(GameLogicTypeFullName);
                if (type != null)
                {
                    return Activator.CreateInstance(type) as GameLogic;
                }
            }

            return null;
        }

    }

    public class ConfigKey : IAssetKey
    {
        public string Key
        {
            get;
        }

        public ConfigKey(string key)
        {
            Key = key;
        }
    }
}
