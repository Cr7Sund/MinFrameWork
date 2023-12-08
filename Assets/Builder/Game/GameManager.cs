// using Cr7Sund.Framework.Api;
// using UnityEngine;
// namespace Cr7Sund
// {
//     public class GameManager
//     {
//         private static GameManager _instance;
//
//         private GameObject _go;
//         private Launch _launch;
//         private GameStatus _status ;
//
//         public static GameManager Instance
//         {
//             get
//             {
//                 return _instance ??= new GameManager();
//             }
//         }
//
//
//         public GameManager()
//         {
//             _status = GameStatus.Closed;
//         }
//         
//         
//         public void Start()
//         {
//             switch (_status)
//             {
//                 case GameStatus.Started:
//                 {
//                     Log.Fatal("GameManager::Start  App已启动...");
//                     break;
//                 }
//                 case GameStatus.Restarting:
//                 {
//                     Log.Fatal("GameManager::Start  App正在重启中...");
//                     break;
//                 }
//                 case GameStatus.Closing:
//                 {
//                     Log.Fatal("GameManager::Start  App正在关闭中...");
//                     break;
//                 }
//                 case GameStatus.Closed:
//                 {
//                     DoStart();
//                     _status = GameStatus.Started;
//                     break;
//                 }
//                 default:
//                     break;
//             }
//         }
//
//         /// <summary>
//         /// 重启
//         /// </summary>
//         public IPromise Restart()
//         {
//             switch (_status)
//             {
//                 case GameStatus.Started:
//                 {
//                     _status = GameStatus.Restarting;
//                     var assetAsync = Shutdown();
//                     if (assetAsync == null)
//                     {
//                         DoStart();
//                         _status = GameStatus.Started;
//                     }
//                     else
//                     {
//                         assetAsync.Then(a =>
//                         {
//                             DoStart();
//                             _status = GameStatus.Started;
//                         });
//                     }
//                     return assetAsync;
//                 }
//                 case GameStatus.Restarting:
//                 {
//                     Log.Warn("GameManager::Restart  App正在重启中....");
//                     return null;
//                 }
//                 case GameStatus.Closing:
//                 {
//                     Log.Warn("GameManager::Restart  App正在关闭中....");
//                     return null;
//                 }
//                 case GameStatus.Closed:
//                 {
//                     DoStart();
//                     _status = GameStatus.Started;
//                     return null;
//                 }
//                 default:
//                     return null;
//             }
//         }
//
//         /// <summary>
//         /// 关闭
//         /// </summary>
//         public IPromise Close()
//         {
//             switch (_status)
//             {
//                 case GameStatus.Started:
//                 {
//                     _status = GameStatus.Closing;
//
//                     var assetAsync = Shutdown();
//                     if (assetAsync == null)
//                         _status = GameStatus.Closed;
//                     else
//                         assetAsync.Then(() => _status = GameStatus.Closed);
//                     return assetAsync;
//                 }
//                 case GameStatus.Restarting:
//                 {
//                     Log.Fatal("GameManager::Close  App正在关闭中");
//                     return null;
//                 }
//                 case GameStatus.Closing:
//                 {
//                     Log.Fatal("GameManager::Close  App正在关闭中");
//                     return null;
//                 }
//                 case GameStatus.Closed:
//                 {
//                     Log.Fatal("GameManager::Close  App还未启动...");
//                     return null;
//                 }
//                 default:
//                     return null;
//             }
//         }
//
//         private void DoStart()
//         {
//             _go = new GameObject("Root");
//
//             KApp.Root = _go.transform;
//             KApp.AppOperator = Instance;
//
//             _launch = _go.AddComponent<Launch>();
//             Object.DontDestroyOnLoad(_go);
//         }
//
//
//         private IPromise Shutdown()
//         {
//             _status = GameStatus.Closing;
//             return _launch.Dispose()
//                  .Then(Dispose);
//         }
//
//         private void Dispose()
//         {
//             Object.Destroy(_go);
//             _launch = null;
//             _go = null;
//
//             System.GC.Collect();
//         }
//
//     }
//     
//     
//     public enum GameStatus
//     {
//         Started,
//         Restarting,
//         Closing,
//         Closed
//     }
// }
