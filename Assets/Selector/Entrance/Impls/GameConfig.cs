using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cr7Sund.Config
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

        public GameLogic.GameLogic GetLogic()
        {
            if (!string.IsNullOrEmpty(AssemblyInfo) &&
                !string.IsNullOrEmpty(GameLogicTypeFullName))
            {
                var assembly = Assembly.Load(AssemblyInfo);
                var type = assembly.GetType(GameLogicTypeFullName);
                if (type != null)
                {
                    return Activator.CreateInstance(type) as GameLogic.GameLogic;
                }
            }

            return null;
        }

    }


}
