using System;

namespace Cr7Sund.Framework.Tests
{
    public class PostConstructSimple
	{

		public static int PostConstructCount  { get; set ; }

		public PostConstructSimple()
		{
		}

		[PostConstruct]
		public void MultiplyBy2()
		{
			PostConstructCount++;
		}
	}
}

