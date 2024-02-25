using UnityEngine;

namespace Cr7Sund.Selector.Impl
{
    public class GameEntrance
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            Console.Init(new InternalLogger());

            Log.Info(LogChannel.Entrance, "Before SplashScreen is shown and before the first scene is loaded.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Log.Info(LogChannel.Entrance, "First scene loading: Before Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Log.Info(LogChannel.Entrance, "First scene loaded: After Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized()
        {
            Log.Info(LogChannel.Entrance, "Runtime initialized: First scene loaded: After Awake is called.");

            if (GameMgr.Instance.Status == Api.GameStatus.Started)
            {
                GameMgr.Instance.Restart();
            }
            else
            {
                GameMgr.Instance.Start();
            }
        }
    }
}