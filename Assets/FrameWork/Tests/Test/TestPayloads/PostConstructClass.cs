using System;

namespace Cr7Sund.Framework.Tests
{
    public class PostConstructClass
    {
        [Inject]
        public float floatVal ;

        public PostConstructClass()
        { }

        [PostConstruct]
        public void MultiplyBy2()
        {
            floatVal *= 2f;
        }
    }
}

