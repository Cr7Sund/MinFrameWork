namespace Cr7Sund.Selector.Api
{
    public interface IGameMgr
    {
        void Start();
        PromiseTask Close();
        PromiseTask Restart();
    }
}