// using System;
// using UnityEngine.Profiling;

// namespace Cr7Sund.Logger
// {
//     public class UnityLogProvider : ILogProvider, IDisposable
//     {
//         private Serilog.Core.Logger _logger;
//         private Enum _logChannel;

//         private string chanelStr;

//         public void Dispose()
//         {
//             _logger.Dispose();
//         }

//         public virtual void Init(LogSinkType logSinkType, string logChannel)
//         {
//             _logChannel = logChannel;
//             chanelStr = _logChannel.ToString().Size(10).Color("#504646");

//             Profiler.enableAllocationCallstacks = true;

//             var loggerConfiguration = new LoggerConfiguration()
//                                             .MinimumLevel.Verbose();

//             if ((logSinkType & LogSinkType.Local) == LogSinkType.Local)
//             {
//                 loggerConfiguration = loggerConfiguration.WriteTo.Unity3D();
//             }
//             if ((logSinkType & LogSinkType.File) == LogSinkType.File)
//             {
//                 loggerConfiguration = loggerConfiguration.WriteTo.File("log.txt",
//                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
//             }

//             _logger = loggerConfiguration.CreateLogger();
//         }

//         public void WriteLine(LogLevel logLevel, string message)
//         {
//             switch (logLevel)
//             {
//                 case LogLevel.Trace:
//                     _logger.Verbose(message);
//                     break;
//                 case LogLevel.Debug:
//                     _logger.Debug(message);
//                     break;
//                 case LogLevel.Info:
//                     _logger.Information(message);
//                     break;
//                 case LogLevel.Warn:
//                     _logger.Warning(message);
//                     break;
//                 case LogLevel.Error:
//                     _logger.Error(message);
//                     break;
//                 case LogLevel.Fatal:
//                     _logger.Fatal(message);
//                     break;
//                 default:
//                     break;
//             }
//         }

//         public void WriteLine<T0>(LogLevel logLevel, string message, T0 propertyValue0)
//         {
//             switch (logLevel)
//             {
//                 case LogLevel.Trace:
//                     _logger.Verbose(message, propertyValue0);
//                     break;
//                 case LogLevel.Debug:
//                     _logger.Debug(message, propertyValue0);
//                     break;
//                 case LogLevel.Info:
//                     _logger.Information(message, propertyValue0);
//                     break;
//                 case LogLevel.Warn:
//                     _logger.Warning(message, propertyValue0);
//                     break;
//                 case LogLevel.Error:
//                     _logger.Error(message, propertyValue0);
//                     break;
//                 case LogLevel.Fatal:
//                     _logger.Fatal(message, propertyValue0);
//                     break;
//                 default:
//                     break;
//             }
//         }

//         public void WriteLine<T0, T1>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1)
//         {
//             switch (logLevel)
//             {
//                 case LogLevel.Trace:
//                     _logger.Verbose(message, propertyValue0, propertyValue1);
//                     break;
//                 case LogLevel.Debug:
//                     _logger.Debug(message, propertyValue0, propertyValue1);
//                     break;
//                 case LogLevel.Info:
//                     _logger.Information(message, propertyValue0, propertyValue1);
//                     break;
//                 case LogLevel.Warn:
//                     _logger.Warning(message, propertyValue0, propertyValue1);
//                     break;
//                 case LogLevel.Error:
//                     _logger.Error(message, propertyValue0, propertyValue1);
//                     break;
//                 case LogLevel.Fatal:
//                     _logger.Fatal(message, propertyValue0, propertyValue1);
//                     break;
//                 default:
//                     break;
//             }
//         }

//         public void WriteLine<T0, T1, T2>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
//         {
//             switch (logLevel)
//             {
//                 case LogLevel.Trace:
//                     _logger.Verbose(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 case LogLevel.Debug:
//                     _logger.Debug(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 case LogLevel.Info:
//                     _logger.Information(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 case LogLevel.Warn:
//                     _logger.Warning(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 case LogLevel.Error:
//                     _logger.Error(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 case LogLevel.Fatal:
//                     _logger.Fatal(message, propertyValue0, propertyValue1, propertyValue2);
//                     break;
//                 default:
//                     break;
//             }
//         }
//     }
// }