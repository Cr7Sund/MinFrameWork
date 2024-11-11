namespace Cr7Sund.LifeTime
{
    public interface ITransitionContainer
    {
        public ITransition GetTransition(IRouteKey assetKey);
    }
}