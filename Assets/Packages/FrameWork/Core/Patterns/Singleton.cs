using System;

namespace Cr7Sund.Patterns
{
    public class Singleton<T> : IInitialize where T : Singleton<T>, new()
    {
        private static T instance;

        public bool IsInit { get; private set; }


        static Singleton()
        {
            Singleton<T>.instance = default(T);
        }


        public virtual void Init()
        {
            IsInit = true;
        }

        public virtual void Dispose()
        {
            IsInit = false;
            Singleton<T>.instance = default(T);
        }

        public static T Instance
        {
            get { return GetInstance(); }
        }


        public static T GetInstance()
        {
            if (null == Singleton<T>.instance)
            {
                Singleton<T>.instance = Activator.CreateInstance<T>();
                Singleton<T>.instance.Init();
            }
            return Singleton<T>.instance;
        }
    }
}
