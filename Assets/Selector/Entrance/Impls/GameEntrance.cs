using UnityEngine;

namespace Cr7Sund.Selector.Impl
{
    public class GameEntrance
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            EntranceConsole.Init(InternalLoggerFactory.Create("GameEntrance"));
            Console.Init(InternalLoggerFactory.Create("FrameWork"));

            EntranceConsole.Info("Before SplashScreen is shown and before the first scene is loaded.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            EntranceConsole.Info("First scene loading: Before Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            EntranceConsole.Info("First scene loaded: After Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized()
        {
            EntranceConsole.Info("Runtime initialized: First scene loaded: After Awake is called.");

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