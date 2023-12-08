// using UnityEngine;
// namespace Cr7Sund
// {
//     internal class GameEntrance
//     {
//         /// <summary>
//         /// 程序入口
//         /// </summary>
//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//         internal static void Access()
//         {
//             Log.Initialize();
//
//             // PLAN : Check Update
//             // PLAN : VAILD CONFIG LOAD
//             int code = IsInvalid(config);
//
//
//             if (AppManager.Instance != null)
//             {
//                 AppManager.Instance.Restart();
//             }
//             else
//             {
//                 // App 启动...
//                 AppManager.Instance.Start();
//             }
//         }
//     }
// }
