using System.Text.RegularExpressions;
namespace Cr7Sund
{
    public class AssertHelper
    {
        public static void IgnoreFailingMessages()
        {
#if UNITY_EDITOR
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        }

        public static void Expect(UnityEngine.LogType logType, Regex regex)
        {
#if UNITY_EDITOR
            UnityEngine.TestTools.LogAssert.Expect(logType, regex);
#endif
        }

        public static void Expect(UnityEngine.LogType logType, string msg)
        {
#if UNITY_EDITOR
            UnityEngine.TestTools.LogAssert.Expect(logType, msg);
#endif
        }
    }
}
