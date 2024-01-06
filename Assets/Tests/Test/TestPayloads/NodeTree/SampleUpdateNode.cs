using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using UnityEngine.PlayerLoop;

namespace Cr7Sund.Framework.Tests
{
    public class SampleUpdateNode : UpdateNode
    {
        public static int UpdateValue = 0;
        public static int LateUpdateValue = 0;

        public SampleUpdateNode()
        {
            _context = new SampleContext();
        }


        public void AssignContext(IContext context)
        {
            _context = context;
        }

        protected override void OnUpdate(int milliseconds)
        {
            base.OnUpdate(milliseconds);
            UpdateValue += milliseconds;
        }

        protected override void OnLateUpdate(int milliseconds)
        {
            base.OnLateUpdate(milliseconds);
            LateUpdateValue += milliseconds;
        }


        #region Test
        public int UpdateChildCount => _updateList.Count;
        public int LateUpdateChildCount => _lateUpdatesList.Count;
        #endregion
    }
}
