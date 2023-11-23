namespace Cr7Sund.Logger
{
    /// <summary>
    ///     具有强制写入功能
    /// </summary>
    internal interface ILogFlushable
    {
        void Flush(LogType logtype);
    }
}
