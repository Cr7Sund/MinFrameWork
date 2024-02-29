using System;
using System.IO;
using UnityEngine;
namespace Cr7Sund.Logger
{
    internal static class LogFileUtil
    {

        private static readonly string _dataPath = Application.dataPath;

#if UNITY_EDITOR
        private static readonly string _streamingAssetsPath = Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets/StreamAssets";
#else
        private static readonly string _streamingAssetsPath = Application.streamingAssetsPath;
#endif

#if UNITY_EDITOR
        private static readonly string _persistentDataPath = Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets";
#elif UNITY_STANDALONE
        private static readonly string _persistentDataPath = Application.dataPath;
#else
        private static readonly string _persistentDataPath = Application.persistentDataPath;
#endif

#if UNITY_EDITOR
        public static string LogDirector = Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets/Logs";
#else
        public static string LogDirector = _persistentDataPath + "/Logs";
#endif

        private static readonly string _configPath = "Configs/Log.bytes";

        #region 代码日志
        private static readonly string _codeMMF = "codelogmemory.bytes";
        private static readonly string _codeDirector = Path.Combine(LogDirector, "Codes");
        private static readonly string _codeMMFPath = Path.Combine(LogDirector, _codeMMF);
        #endregion

        #region 事件日志
        private static readonly string _eventMMF = "eventlogmemory.bytes";
        private static readonly string _eventDirector = Path.Combine(LogDirector, "Events");
        private static readonly string _eventMMFPath = Path.Combine(LogDirector, _eventMMF);
        #endregion

        /// <summary> 日志映射的内存上限 </summary>
        public static readonly int LogMemorySize = 1024 * 1024;

        /// <summary>
        ///     获取对应类型日志的MMF名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMMFileMapName(FileLogType type)
        {
            switch (type)
            {
                case FileLogType.Code:
                    return "code";
                case FileLogType.Event:
                    return "event";
                default:
                    throw new Exception("未支持的日志类型");
            }
        }

        /// <summary>
        ///     获取对应日志文件所在的文件夹
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileDirector(FileLogType type)
        {
            switch (type)
            {
                case FileLogType.Code:
                    return _codeDirector;
                case FileLogType.Event:
                    return _eventDirector;
                default:
                    throw new Exception("未支持的日志类型");
            }
        }

        /// <summary>
        ///     拼接对应日志本地缓存的路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetCopyFilePath(FileLogType type, string file)
        {
            switch (type)
            {
                case FileLogType.Code:
                    return Path.Combine(_codeDirector, $"{file}.bytes");
                case FileLogType.Event:
                    return Path.Combine(_eventDirector, $"{file}.bytes");
                default:
                    throw new Exception("未支持的日志类型");
            }
        }

        /// <summary>
        ///     获取对应日志MMF所在路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMemoryPath(FileLogType type)
        {
            switch (type)
            {
                case FileLogType.Code:
                    return _codeMMFPath;
                case FileLogType.Event:
                    return _eventMMFPath;
                default:
                    throw new Exception("未支持的日志类型");

            }
        }

        /// <summary>
        ///     获取游戏包里的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="includeUpdate"></param>
        /// <returns></returns>
        public static string GetLogConfigPath()
        {
            //找不到再从内置资源里取
            if (Application.platform == RuntimePlatform.Android)
                return $"{_dataPath}!assets/{_configPath}";
            return Path.Combine(_streamingAssetsPath, _configPath);
        }
    }
}
