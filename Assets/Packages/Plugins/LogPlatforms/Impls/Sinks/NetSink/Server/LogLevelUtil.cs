namespace Cr7Sund.Logger
{
    public class LogLevelUtil
    {
        public static string GetLogLevel(ushort logLevel, int length = 3)
        {
            return _titleCaseLevelMap[logLevel][length];
        }

        static readonly string[][] _titleCaseLevelMap = {
        new []{ "V", "Vb", "Vrb", "Verb", "Verbo", "Verbos", "Verbose" },
        new []{ "D", "De", "Dbg", "Dbug", "Debug" },
        new []{ "I", "In", "Inf", "Info", "Infor", "Inform", "Informa", "Informat", "Informati", "Informatio", "Information" },
        new []{ "W", "Wn", "Wrn", "Warn", "Warni", "Warnin", "Warning" },
        new []{ "E", "Er", "Err", "Eror", "Error" },
        new []{ "F", "Fa", "Ftl", "Fatl", "Fatal" }
    };
    }
}