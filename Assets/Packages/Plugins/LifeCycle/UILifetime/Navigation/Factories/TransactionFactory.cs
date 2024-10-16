using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Navigation
{
    public static class TransactionFactory 
    {
        private static readonly Factory<ITransaction> _pool = new();

        public static void Return(ITransaction route)
        {
            _pool.ReturnInstance(route);
        }
        public static ITransaction Create<TInstance>()where TInstance : ITransaction, new()
        {
            return _pool.CreateInstance<TInstance>();
        }
    }
}