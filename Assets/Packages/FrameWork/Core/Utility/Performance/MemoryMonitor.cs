using System.Collections.Generic;

namespace Cr7Sund.Performance
{
    public delegate void CleanMemoryDelegate();

    public static class MemoryMonitor
    {
        private static HashSet<CleanMemoryDelegate> _delegateRegisters = new HashSet<CleanMemoryDelegate>();


        public static void CleanMemory()
        {
            Console.Info("Clean Memory!!");

            foreach (var register in _delegateRegisters)
            {
                try
                {
                    register?.Invoke();
                }
                catch (System.Exception e)
                {
                    Console.Fatal($"Memory Warning Delegate Exception: Message: {e.Message}, StackTrace: {e.StackTrace}");
                }
            }
        }

        public static void Register(CleanMemoryDelegate register)
        {
            if (register == null)
                return;

            if (!_delegateRegisters.Contains(register))
                _delegateRegisters.Add(register);
        }


        public static void UnRegister(CleanMemoryDelegate unRegister)
        {
            if (unRegister == null)
                return;

            if (_delegateRegisters.Contains(unRegister))
                _delegateRegisters.Remove(unRegister);
        }

        public static void Dispose()
        {
            _delegateRegisters.Clear();
        }
    }
}
