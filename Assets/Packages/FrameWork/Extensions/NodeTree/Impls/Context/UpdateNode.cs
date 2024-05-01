using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{

    public abstract class UpdateNode : Node, IUpdatable, ILateUpdate
    {
        protected List<UpdateNode> _updateList = new();
        protected List<UpdateNode> _lateUpdatesList = new();


        public UpdateNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        protected override void OnAddChild(INode child)
        {
            base.OnAddChild(child);

            if (child == null)
            {
                return;
            }

            if (child is IUpdatable)
            {
                var updateNode = child as UpdateNode;
                if (MacroDefine.IsDebug)
                {
                    AssertUtil.IsFalse(_updateList.Contains(updateNode));
                }
                _updateList.Add(updateNode);
            }

            if (child is ILateUpdate)
            {
                var updateNode = child as UpdateNode;
                if (MacroDefine.IsDebug)
                {
                    AssertUtil.IsFalse(_lateUpdatesList.Contains(updateNode));
                }

                _lateUpdatesList.Add(updateNode);
            }
        }

        protected override void OnRemoveChild(INode child)
        {
            base.OnRemoveChild(child);

            if (child == null)
            {
                return;
            }

            if (child is IUpdatable)
            {
                var updateNode = child as UpdateNode;
                if (_updateList.Contains(updateNode))
                    _updateList.Remove(updateNode);
            }

            if (child is ILateUpdate)
            {
                var updateNode = child as UpdateNode;
                if (_lateUpdatesList.Contains(updateNode))
                    _lateUpdatesList.Remove(updateNode);
            }
        }

        public void Update(int elapse)
        {
            if (!IsStarted || !IsActive)
                return;

            int updateListCount = _updateList.Count;
            // in case remove list on Update
            for (int i = updateListCount - 1; i >= 0; i--)
            {
                _updateList[i].Update(elapse);
            }

            OnUpdate(elapse);
        }

        public void LateUpdate(int elapse)
        {
            if (!IsStarted || !IsActive)
                return;

            int count = _lateUpdatesList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                _updateList[i].LateUpdate(elapse);
            }
            OnLateUpdate(elapse);
        }

        protected virtual void OnUpdate(int milliseconds) { }
        protected virtual void OnLateUpdate(int milliseconds) { }


    }
}
