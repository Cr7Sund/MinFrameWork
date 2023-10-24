using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Tests
{
	class TestContextSubclass : Context
	{
		public static string INIT_VALUE = "Zaphod";
		public static string MAPPING_VALUE = "Ford Prefect";
		public static string LAUNCH_VALUE = "Arthur Dent";

		private string _testValue = INIT_VALUE;
		public string testValue
		{
			get { return _testValue; }
		}

		public TestContextSubclass(object view) : base(view) { }
		public TestContextSubclass(object view, bool autoMapping) : base(view, autoMapping) { }
		public TestContextSubclass(object view, ContextStartupFlags flags) : base(view, flags) { }


		protected override void MapBindings()
		{
			base.MapBindings();
			_testValue = MAPPING_VALUE;
		}

		public override void Launch()
		{
			base.Launch();
			_testValue = LAUNCH_VALUE;
		}
	}

}