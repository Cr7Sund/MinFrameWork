using System.Collections.Generic;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    /// <summary>
    /// 支持Update、LateUpdate的适配节点
    /// </summary>
    public abstract class UpdateNode : Node, IUpdate, ILateUpdate
    {
        private List<UpdateNode> _updateList;
        private List<UpdateNode> _lateUpdatesList;


        public UpdateNode() : base()
        {
            _updateList = new List<UpdateNode>();
            _lateUpdatesList = new List<UpdateNode>();
        }


        protected override void OnAddChild(Node child)
        {
            base.OnAddChild(child);

            if (child == null)
            {
                return;
            }

            if (child is IUpdate)
            {
                var updateNode = child as UpdateNode;
                if (!_updateList.Contains(updateNode))
                    _updateList.Add(updateNode);
            }

            if (child is ILateUpdate)
            {
                var updateNode = child as UpdateNode;
                if (!_lateUpdatesList.Contains(updateNode))
                    _lateUpdatesList.Add(updateNode);
            }
        }

        protected override void OnRemoveChild(Node child)
        {
            base.OnRemoveChild(child);

            if (child == null)
            {
                return;
            }


            if (child is IUpdate)
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

            for (int i = 0; i < _updateList.Count; i++)
            {
                _updateList[i].Update(elapse);
            }

            OnUpdate(elapse);
        }

        public void LateUpdate(int elapse)
        {
            if (!IsStarted || !IsActive)
                return;

            for (int i = 0; i < _updateList.Count; i++)
            {
                _updateList[i].LateUpdate(elapse);
            }
            OnLateUpdate(elapse);
        }

        protected virtual void OnUpdate(int milliseconds) { }
        protected virtual void OnLateUpdate(int milliseconds) { }

    }
}
