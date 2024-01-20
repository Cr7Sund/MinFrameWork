using UnityEngine;

namespace Cr7Sund.Selector.Impl
{
    public class GameEntrance
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            Debug.Init(new InternalLogger());


            Debug.Info("Before SplashScreen is shown and before the first scene is loaded.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Debug.Info("First scene loading: Before Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Debug.Info("First scene loaded: After Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized()
        {
            Debug.Info("Runtime initialized: First scene loaded: After Awake is called.");

            if (GameMgr.Instance.Status == Api.GameStatus.Started)
            {
                GameMgr.Instance.Restart();
            }
            else
            {
                // GameMgr.Instance.Start();
            }
        }
    }
}