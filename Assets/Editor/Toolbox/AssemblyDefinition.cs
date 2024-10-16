using System.Collections.Generic;
namespace Cr7Sund.Editor
{
    [System.Serializable]
    public class AssemblyDefinition
    {
        public string name;
        public string rootNamespace;
        public List<string> references;
        public List<string> includePlatforms;
        public List<string> excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public List<string> precompiledReferences;
        public bool autoReferenced;
        public List<string> defineConstraints;
        public List<string> versionDefines;
        public bool noEngineReferences;
    }
}
