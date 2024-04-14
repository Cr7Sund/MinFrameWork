namespace Cr7Sund.Selector.Apis
{
    public interface IGameLogic : IDestroyAsync
    {
        void Init();
        PromiseTask Run();
        void Update(int millisecond);
        void LateUpdate(int millisecond);
    }
}