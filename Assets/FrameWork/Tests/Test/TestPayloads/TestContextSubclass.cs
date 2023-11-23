using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
namespace Cr7Sund.Framework.Tests
{
    internal class TestContextSubclass : Context
    {
        public static string INIT_VALUE = "Zaphod";
        public static string MAPPING_VALUE = "Ford Prefect";
        public static string LAUNCH_VALUE = "Arthur Dent";

        public TestContextSubclass(object view) : base(view) { }
        public TestContextSubclass(object view, bool autoMapping) : base(view, autoMapping) { }
        public TestContextSubclass(object view, ContextStartupFlags flags) : base(view, flags) { }
        public string testValue
        {
            get;
            private set;
        } = INIT_VALUE;


        protected override void MapBindings()
        {
            base.MapBindings();
            testValue = MAPPING_VALUE;
        }

        public override void Launch()
        {
            base.Launch();
            testValue = LAUNCH_VALUE;
        }
    }

}
