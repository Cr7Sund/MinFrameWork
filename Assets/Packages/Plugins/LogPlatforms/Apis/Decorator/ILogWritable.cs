namespace Cr7Sund.Logger
{
    internal interface ILogWritable
    {
        void Write(string type, string id, object obj);
        void Flush();
    }
}
