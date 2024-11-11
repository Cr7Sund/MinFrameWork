using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{

    public abstract class UpdatableNode : Node, IUpdatable, ILateUpdatable
    {
        public void Update(int elapse)
        {
            if (!IsStarted || !IsActive)
                return;

            // in case remove list on Update
            for (int i = ChildCount - 1; i >= 0; i--)
            {
                var child = GetChild(i);
                if (child is IUpdatable updatable)
                {
                    updatable.Update(elapse);
                }
            }

            OnUpdate(elapse);
        }

        public void LateUpdate(int elapse)
        {
            if (!IsStarted || !IsActive)
                return;

            for (int i = ChildCount - 1; i >= 0; i--)
            {
                var child = GetChild(i);
                if (child is ILateUpdatable updatable)
                {
                    updatable.LateUpdate(elapse);
                }
            }
            OnLateUpdate(elapse);
        }

        protected virtual void OnUpdate(int milliseconds) { }
        protected virtual void OnLateUpdate(int milliseconds) { }
        
    }
}
