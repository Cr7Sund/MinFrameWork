using Cr7Sund.Logger;

namespace Cr7Sund
{
    public static class LogProviderFactory
    {
        public static ILogProvider Create()
        {
#if UNITY_EDITOR
            if (MacroDefine.IsMainThread && !UnityEditor.EditorApplication.isPlaying)
            {
                return new UnityEditorLogProvider();
            }
#endif

            return new SerilogProvider();
        }
    }
}
