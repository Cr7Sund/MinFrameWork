namespace Cr7Sund.Server.UI.Api
{
    public interface IUIModule
    {
        /// <summary>
        /// current ui containers stack count
        /// </summary>
        int OperateNum { get; }

        PromiseTask CloseAll();
    }

}
