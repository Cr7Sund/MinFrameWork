namespace Cr7Sund.Selector.Apis
{
    public interface IGameLogic : IDestroyAsync, IUpdatable, ILateUpdatable
    {
        PromiseTask Run();
    }
}