using System;

namespace Cr7Sund.Patterns
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance;

        static Singleton()
        {
            Singleton<T>.instance = default(T);
        }

        public virtual void Init()
        {
        }

        public virtual void Dispose()
        {
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
