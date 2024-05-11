namespace Cr7Sund.NodeTree.Impl
{
    public abstract class UpdateController : BaseController, IUpdatable
    {
        public void Update(int millisecond)
        {
            OnUpdate(millisecond);
        }

        protected abstract void OnUpdate(int millisecond);
    }
}
