using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Tests
{
    public class SampleGameLogic : GameLogic.GameLogic
    {
        protected override GameBuilder CreateBuilder()
        {
            return new SampleGameBuilder();
        }
    }
}