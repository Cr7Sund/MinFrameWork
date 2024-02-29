namespace Cr7Sund.Logger
{
    /// <summary>
    ///     具有强制写入功能
    /// </summary>
    internal interface ILogFlushable
    {
        void Flush(FileLogType logtype);
    }
    
    public enum FileLogType : byte
    {
        /// <summary> 代码日志 </summary>
        Code = 1,

        /// <summary> 埋点日志 </summary>
        Event = 1 << 1
    }

}
