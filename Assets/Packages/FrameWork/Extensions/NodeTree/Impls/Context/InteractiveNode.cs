
// using Cr7Sund.Framework.Api;
// using Cr7Sund.Framework.Impl;

// namespace Cr7Sund.NodeTree.Impl
// {
//     /// <summary>
//     /// 可交互Node
//     /// </summary>
//     public abstract class InteractiveNode : UpdateNode, IObservable
//     {
//         private readonly EventDispatcher _eventDispatcher;


//         public InteractiveNode()
//         {
//             _eventDispatcher = new EventDispatcher();
//         }

//         public void AddListener(object evt, EventCallback callback)
//         {
//             _eventDispatcher.Dispatch(evt, callback);
//         }

//         public void AddListener(object evt, EmptyCallback callback)
//         {
//             _eventDispatcher.Dispatch(evt, callback);
//         }

//         public void RemoveListener(object evt, EventCallback callback)
//         {
//             _eventDispatcher.RemoveListener(evt, callback);
//         }

//         public void RemoveListener(object evt, EmptyCallback callback)
//         {
//             _eventDispatcher.RemoveListener(evt, callback);
//         }
//     }
// }
