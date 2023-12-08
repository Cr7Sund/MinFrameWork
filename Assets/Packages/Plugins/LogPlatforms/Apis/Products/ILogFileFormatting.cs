namespace Cr7Sund.Logger
{
    internal interface ILogFileFormatting
    {
        byte[] Formatting(byte[] buffer);
        byte[] UnFormatting(byte[] buffer);
    }
}
