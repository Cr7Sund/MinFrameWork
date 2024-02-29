using LitJson;
using System;
using System.Text;
using System.Threading;
namespace Cr7Sund.Logger
{
    internal abstract class LogWriter<TMsg> : ILogWritable
        , IDisposable
    {
        protected ILogFileFormatting _formatter;
        protected MMFile _mmFile;

        public object _syncRoot;
        protected JsonData jsonData = new JsonData();

        public LogWriter(ILogFileFormatting formatter, MMFile mmFile)
        {
            _formatter = formatter;
            _mmFile = mmFile;
        }
        protected abstract FileLogType FileLogType { get; }
        private object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    //如果_syncRoot和null相等，将new object赋值给 _syncRoot
                    //Interlocked.CompareExchange方法保证多个线程在使用 syncRoot时是线程安全的
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        public void Dispose()
        {
            _mmFile.Dispose();
            _mmFile = null;
        }

        public void Write(string type, string id, object obj)
        {
            lock (SyncRoot)
            {
                string msg = Formatting(type, id, (TMsg)obj);
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                Write(buffer, 0, buffer.Length);
                buffer = Encoding.UTF8.GetBytes("\n");
                Write(buffer, 0, buffer.Length);
            }
        }

        public void Flush()
        {
            lock (SyncRoot)
            {
                WriteToFile();
                _mmFile.Reset();
            }
        }

        protected abstract string Formatting(string type, string id, TMsg obj);

        private void WriteToFile()
        {
            if (_mmFile.IsWritable()) return;
            byte[] buffer = _mmFile.ReadAll();
            buffer = _formatter.Formatting(buffer);
            LogFileManager.Append(LogFileUtil.GetCopyFilePath(FileLogType, ((long)(DateTime.UtcNow - FileLogDecorator.StandardTime).TotalSeconds).ToString()), buffer, 0, buffer.Length);
        }

        private void Write(byte[] buffer, int offset, int length)
        {
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            try
            {
                _mmFile.Write(buffer, offset, length);
            }
            catch (MMFileOverflowException e)
            {
                WriteToFile();
                _mmFile.Reset();
                Write(buffer, offset, length);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
        }
    }
}
