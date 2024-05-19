namespace Cr7Sund.Selector.Apis
{
    public interface IGameLogic : IDestroyAsync, IUpdatable, ILateUpdate
    {
        void Init();
        PromiseTask Run();
    }
}