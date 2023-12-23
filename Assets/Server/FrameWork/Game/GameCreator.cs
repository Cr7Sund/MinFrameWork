namespace Cr7Sund.Server.Impl
{
    public class GameCreator 
    {
        public GameNode Create()
        {
            var builder = new GameBuilder();
            GameDirector.Construct(builder);
            return builder.GetProduct();
        }
    }
}