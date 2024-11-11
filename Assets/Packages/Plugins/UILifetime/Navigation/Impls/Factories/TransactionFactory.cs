using Cr7Sund.FrameWork.Util;
using Cr7Sund.IocContainer;
namespace Cr7Sund.LifeTime
{
    public class TransactionFactory
    {
        private readonly Factory<ITransaction> _pool = new();

        public void Return(ITransaction transaction, IContext context)
        {
            context.Inject(transaction);
            _pool.ReturnInstance(transaction);
        }

        public ITransaction Create<TInstance>(IContext context) where TInstance : ITransaction, new()
        {
            var transaction = _pool.CreateInstance<TInstance>();
            context.Inject(transaction);
            return transaction;
        }
    }
}
