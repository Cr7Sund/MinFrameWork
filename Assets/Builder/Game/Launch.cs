// namespace Cr7Sund
// {
//     public class Launch
//     {
//         public class Launch : MonoBehaviour
//         {
//             private KailashFramework _kailashFramework;
//             private bool _dispose;
//
//             internal void Awake()
//             {
//                 _kailashFramework = new KailashFramework();
//             }
//
//             private void Start()
//             {
//                 _kailashFramework.Start();
//             }
//
//             private void Update()
//             {
//                 ++KApp.FrameCount;
//
//                 if (_dispose) return;
//
//                 try
//                 {
//                     _kailashFramework.Update();
//                 }
//                 catch
//                 {
//                     if (!_dispose) throw;
//                 }
//             }
//
//             private void LateUpdate()
//             {
//                 if (_dispose) return;
//                 try
//                 {
//                     _kailashFramework.LateUpdate();
//                 }
//                 catch
//                 {
//                     if (!_dispose) throw;
//                 }
//             }
//
//             internal void Dispose()
//             {
//                 if (!_dispose)
//                 {
//                     _dispose = true;
//                     _kailashFramework.Dispose();
//                 }
//             }
//
//             internal void OnApplicationQuit()
//             {
//                 Dispose();
//                 Log.Dispose();
//             }
//
//         }
//     }
//
// }
