using System;
using System.Diagnostics;
using Cr7Sund.Logger;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Cr7Sund
{
    /// <summary>
    ///     Logger utility class providing different logging levels for Unity.
    /// </summary>
    public static class Log
    {
        private static ILogDecorator _logDecorator;

        /// <summary>
        ///     Gets the instance of the log decorator, creating it if necessary.
        /// </summary>
        private static ILogDecorator LogDecorator
        {
            get
            {
                // Lazy initialization of log decorator using LogDecoratorCreator.
                return _logDecorator ??= LogDecoratorCreator.Create();
            }
        }

        #region Utility Methods

        /// <summary>
        ///     Formats an exception for logging purposes.
        /// </summary>
        /// <param name="exception">The exception to format.</param>
        /// <returns>A formatted string representing the exception.</returns>
        private static string ParseException(Exception exception)
        {
            return $"{exception.GetType().FullName}: {exception.Message}\nStackTrace: {exception.StackTrace}";
        }

        #endregion

        #region Trace Logging

        /// <summary>
        ///     Log a trace message (only visible in Unity Editor during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Trace, LogChannel.Undefine, format, args);
            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        ///     Log a trace message with a specified log channel (only visible in Unity Editor during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the trace message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Trace, logChannel, format, args);
            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        ///     Log a trace message (only visible in Unity Editor during development).
        /// </summary>
        /// <param name="obj">Object to log</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(object obj)
        {
            string result = LogDecorator.Format(LogLevel.Trace, LogChannel.Undefine, obj.ToString());
            UnityEngine.Debug.Log(result);
        }

        // Additional Trace overloads...

        #endregion

        #region Debug Logging

        /// <summary>
        ///     Log a debug message (visible in Unity Editor and Debug builds during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Debug, LogChannel.Undefine, format, args);
            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        ///     Log a debug message with a specified log channel (visible in Unity Editor and Debug builds during development).
        /// </summary>
        /// <param name="obj">Object to log</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(object obj)
        {
            string result = LogDecorator.Format(LogLevel.Debug, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.Log(result);
        }
        /// <summary>
        ///     Log a debug message with a specified log channel (visible in Unity Editor and Debug builds during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Debug, logChannel, format, args);
            UnityEngine.Debug.Log(result);
        }

        #endregion

        #region Info Logging

        /// <summary>
        ///     Log an info message (visible in all versions during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Info(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Info, LogChannel.Undefine, format, args);
            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        ///     Log an info message (visible in all versions during development).
        /// </summary>
        /// <param name="obj">Object to log</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Info(object obj)
        {
            string result = LogDecorator.Format(LogLevel.Info, LogChannel.Undefine, obj.ToString());
            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        ///     Log an info message (visible in all versions during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Info(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Debug, logChannel, format, args);
            UnityEngine.Debug.Log(result);
        }
        #endregion

        #region Warn Logging

        /// <summary>
        ///     Log a warning message (visible in Unity Editor, Debug builds, and Profiler during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        public static void Warn(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Warn, LogChannel.Undefine, format, args);
            UnityEngine.Debug.LogWarning(result);
        }

        /// <summary>
        ///     Log a warning message (visible in Unity Editor, Debug builds, and Profiler during development).
        /// </summary>
        /// <param name="exception">Exception</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        public static void Warn(Exception exception)
        {
            Warn(null, exception);
        }

        /// <summary>
        ///     Log a warning message (visible in Unity Editor, Debug builds, and Profiler during development).
        /// </summary>
        /// <param name="prefix">Formatted string prefix</param>
        /// <param name="exception">Exception</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        public static void Warn(string prefix, Exception exception)
        {
            if (null == exception)
            {
                Warn("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Warn("{0} {1}", prefix, exceptionStr);
        }

        /// <summary>
        ///     Log a warning message (visible in Unity Editor, Debug builds, and Profiler during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        public static void Warn(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Warn, logChannel, format, args);

            UnityEngine.Debug.LogWarning(result);
        }

        #endregion

        #region Error Logging

        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Error, LogChannel.Undefine, format, args);
            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="obj">Object to log</param>
        /// <param name="context">message context</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(object obj, Object context = null)
        {
            string result = LogDecorator.Format(LogLevel.Error, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.LogError(result, context);
        }

        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="exception">Exception</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(Exception exception)
        {
            Error(null, exception);
        }
        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="exception"></param>
        public static void Error(string prefix, Exception exception)
        {
            if (null == exception)
            {
                Error("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Error("{0} {1}", prefix, exceptionStr);
        }

        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="prefix"></param>
        /// <param name="exception"></param>
        public static void Error(LogChannel logChannel, string prefix, Exception exception)
        {
            if (null == exception)
            {
                Error("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            string result = LogDecorator.Format(LogLevel.Error, logChannel, $"{prefix} {exceptionStr}");
            UnityEngine.Debug.LogError(result);
        }
        /// <summary>
        ///     Log an error message (visible in all versions during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Error, logChannel, format, args);

            UnityEngine.Debug.LogError(result);
        }

        #endregion

        #region Fatal Logging

        /// <summary>
        ///     Log a fatal error message (visible in all versions during development).
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Fatal, LogChannel.Undefine, format, args);
            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        ///     Log a fatal error message (visible in all versions during development).
        /// </summary>
        /// <param name="obj">Object to log</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(object obj)
        {
            string result = LogDecorator.Format(LogLevel.Fatal, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        ///     Log a fatal error message (visible in all versions during development).
        /// </summary>
        /// <param name="prefix">Formatted string prefix</param>
        /// <param name="exception">Exception</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(string prefix, Exception exception)
        {
            if (null == exception)
            {
                Fatal("{0} Exception is null.", prefix);
                return;
            }
            string exceptionStr = ParseException(exception);
            UnityEngine.Debug.LogError(exceptionStr);
        }
        /// <summary>
        ///     Log a fatal error message (visible in all versions during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="prefix"></param>
        /// <param name="exception"></param>
        public static void Fatal(LogChannel logChannel, string prefix, Exception exception)
        {
            if (null == exception)
            {
                Error("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            string result = LogDecorator.Format(LogLevel.Fatal, logChannel, $"{prefix} {exceptionStr}");
            UnityEngine.Debug.LogError(result);
        }
        /// <summary>
        ///     Log a fatal error message (visible in all versions during development).
        /// </summary>
        /// <param name="logChannel">The log channel of the debug message.</param>
        /// <param name="format">Format string</param>
        /// <param name="args">Variable number of format parameters</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        [Conditional(MacroDefine.DEBUG)]
        [Conditional(MacroDefine.PROFILER)]
        [Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(LogChannel logChannel, string format, params object[] args)
        {
            string result = LogDecorator.Format(LogLevel.Fatal, logChannel, format, args);

            UnityEngine.Debug.LogError(result);
        }

        #endregion

        #region Initialization and Cleanup

        /// <summary>
        ///     Initializes the log system.
        /// </summary>
        internal static void Initialize()
        {
            Profiler.enableAllocationCallstacks = true;
            LogDecorator.Initialize();
        }

        /// <summary>
        ///     Disposes of the log system.
        /// </summary>
        public static void Dispose()
        {
            LogDecorator?.Dispose();
            _logDecorator = null;
        }

        #endregion
    }
}
