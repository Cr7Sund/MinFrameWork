using System;
namespace Cr7Sund.FrameWork.Util
{
    public class TimeUtils
    {
        public static long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }
}
