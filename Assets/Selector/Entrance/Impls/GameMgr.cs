using Cr7Sund.Selector.Api;
using Cr7Sund.Patterns;
using UnityEngine;

namespace Cr7Sund.Selector.Impl
{
    public class GameMgr : Singleton<GameMgr>, IGameMgr
    {
        private GameStatus _status = GameStatus.Closed;
        private GameObject _go;
        private GameLauncher _launch;

        public GameStatus Status => _status;

#if UNITY_EDITOR
        public GameMgr()
        {
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;
        }

        private async void PlayModeChange(UnityEditor.PlayModeStateChange change)
        {
            if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                await Close();
            }
        }
#endif
        public void Start()
        {
            switch (_status)
            {
                case GameStatus.Started:
                    {
                        EntranceConsole.Warn("GameMgr::Start  Game already started...");
                        break;
                    }
                case GameStatus.Restarting:
                    {
                        EntranceConsole.Warn("GameMgr::Start  Game is currently restarting...");
                        break;
                    }
                case GameStatus.Closing:
                    {
                        EntranceConsole.Fatal("GameMgr::Start  Game is currently closing...");
                        break;
                    }
                case GameStatus.Closed:
                    {
                        EntranceConsole.Info("GameMgr::Start...");
                        DoStart();
                        _status = GameStatus.Started;
                        break;
                    }
            }
        }

        public async PromiseTask Restart()
        {
            switch (_status)
            {
                case GameStatus.Started:
                    _status = GameStatus.Restarting;
                    EntranceConsole.Warn("GameMgr::Restart  Close game first....");
                    await DoClose();
                    EntranceConsole.Info("GameMgr::ReStart...");
                    DoStart();
                    _status = GameStatus.Started;
                    return;
                case GameStatus.Restarting:
                    EntranceConsole.Warn("GameMgr::Restart  Game is currently restarting....");
                    return;
                case GameStatus.Closing:
                    EntranceConsole.Warn("GameMgr::Restart  Game is currently closing....");
                    return;
                case GameStatus.Closed:
                    EntranceConsole.Info("GameMgr::ReStart...");
                    DoStart();
                    _status = GameStatus.Started;
                    return;
                default:
                    return;
            }
        }

        public async PromiseTask Close()
        {
            switch (_status)
            {
                case GameStatus.Started:
                    {
                        EntranceConsole.Info("GameMgr::Close");
                        _status = GameStatus.Closing;
                        await DoClose();
                        _status = GameStatus.Closed;
                        return;
                    }
                case GameStatus.Restarting:
                    {
                        EntranceConsole.Fatal("GameMgr::Close  Game is currently closing while restarting");
                        return;
                    }
                case GameStatus.Closing:
                    {
                        EntranceConsole.Fatal("GameMgr::Close  Game is currently closing");
                        return;
                    }
                case GameStatus.Closed:
                    {
                        // EntranceConsole.Fatal("GameMgr::Close  Game has not been started yet...");
                        return;
                    }
                default:
                    return;
            }
        }

        private void DoStart()
        {
            _go = new GameObject("Root");
            _launch = _go.AddComponent<GameLauncher>();
            Object.DontDestroyOnLoad(_go);
        }

        private async PromiseTask DoClose()
        {
            await _launch.DestroyAsync();
            Dispose();
        }
    }
}
