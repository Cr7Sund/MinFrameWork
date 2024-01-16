using Cr7Sund.Selector.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Selector.Impl
{
    public class EditorGameLogic : GameLogic
    {

        protected override GameBuilder CreateBuilder()
        {
            return new EditorGameBuilder();
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
    }
}