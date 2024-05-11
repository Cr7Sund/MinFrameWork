namespace Cr7Sund.Selector.Api
{
    internal interface IGameMgr
    {
        void Start();
        PromiseTask Close();
        PromiseTask Restart();
    }
}