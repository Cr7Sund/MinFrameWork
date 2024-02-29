namespace Cr7Sund.Logger.Server
{
    [System.Serializable]
    public class LogEntryData
    {
        public long TimeStamp;
        public ushort Level;
        public string Message;
        public string ExceptionMsg;

        public string Color;
        public string LogChannel;
    }

}