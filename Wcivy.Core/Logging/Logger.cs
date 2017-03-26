using log4net;
using log4net.Config;
using log4net.Core;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Wcivy.Core.Logging
{
    public class Logger
    {
        // 封装log4net，如果要在日志中输出当前类和方法必须用ILogger，不能使用ILog
        private static ILog _log;
        private static ILogger _logger;
        public static readonly Logger Instance = new Logger();
        private Logger()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "log4net.config");
            // 监视配置文件，配置变动会重新加载
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        #region Debug
        public void Debug(string message)
        {
            _log = LogManager.GetLogger(LogLevel.Debug.ToString());
            if (_log.IsDebugEnabled)
            {
                Log(LogLevel.Debug, message);
            }
        }

        public void Debug(string message, Exception exception)
        {
            _log = LogManager.GetLogger(LogLevel.Debug.ToString());
            if (_log.IsDebugEnabled)
            {
                Log(LogLevel.Debug, message, exception);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Debug.ToString());
            if (_log.IsDebugEnabled)
            {
                Log(LogLevel.Debug, format, args);
            }
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Debug.ToString());
            if (_log.IsDebugEnabled)
            {
                Log(LogLevel.Debug, string.Format(format, args), exception);
            }
        }
        #endregion

        #region Info
        public void Info(string message)
        {
            _log = LogManager.GetLogger(LogLevel.Info.ToString());
            if (_log.IsInfoEnabled)
            {
                Log(LogLevel.Info, message);
            }
        }

        public void Info(string message, Exception exception)
        {
            _log = LogManager.GetLogger(LogLevel.Info.ToString());
            if (_log.IsInfoEnabled)
            {
                Log(LogLevel.Info, message, exception);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Info.ToString());
            if (_log.IsInfoEnabled)
            {
                Log(LogLevel.Info, format, args);
            }
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Info.ToString());
            if (_log.IsInfoEnabled)
            {
                Log(LogLevel.Info, string.Format(format, args), exception);
            }
        }
        #endregion

        #region  Warn

        public void Warn(string message)
        {
            _log = LogManager.GetLogger(LogLevel.Warn.ToString());
            if (_log.IsWarnEnabled)
            {
                Log(LogLevel.Warn, message);
            }
        }

        public void Warn(string message, Exception exception)
        {
            _log = LogManager.GetLogger(LogLevel.Warn.ToString());
            if (_log.IsWarnEnabled)
            {
                Log(LogLevel.Warn, message, exception);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Warn.ToString());
            if (_log.IsWarnEnabled)
            {
                Log(LogLevel.Warn, format, args);
            }
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Warn.ToString());
            if (_log.IsWarnEnabled)
            {
                Log(LogLevel.Warn, string.Format(format, args), exception);
            }
        }
        #endregion

        #region  Error
        public void Error(string message)
        {
            _log = LogManager.GetLogger(LogLevel.Error.ToString());
            if (_log.IsErrorEnabled)
            {
                Log(LogLevel.Error, message);
            }
        }

        public void Error(string message, Exception exception)
        {
            _log = LogManager.GetLogger(LogLevel.Error.ToString());
            if (_log.IsErrorEnabled)
            {
                Log(LogLevel.Error, message, exception);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Error.ToString());
            if (_log.IsErrorEnabled)
            {
                Log(LogLevel.Error, format, args);
            }
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Error.ToString());
            if (_log.IsErrorEnabled)
            {
                Log(LogLevel.Error, string.Format(format, args), exception);
            }
        }
        #endregion

        #region  Fatal
        public void Fatal(string message)
        {
            _log = LogManager.GetLogger(LogLevel.Fatal.ToString());
            if (_log.IsFatalEnabled)
            {
                Log(LogLevel.Fatal, message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            _log = LogManager.GetLogger(LogLevel.Fatal.ToString());
            if (_log.IsFatalEnabled)
            {
                Log(LogLevel.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Fatal.ToString());
            if (_log.IsFatalEnabled)
            {
                Log(LogLevel.Fatal, format, args);
            }
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            _log = LogManager.GetLogger(LogLevel.Fatal.ToString());
            if (_log.IsFatalEnabled)
            {
                Log(LogLevel.Fatal, string.Format(format, args), exception);
            }
        }
        #endregion

        #region ILogger
        public void Log(LogLevel logLevel, object message, Exception exception = null)
        {
            Level level;
            switch (logLevel)
            {
                case LogLevel.Debug:
                    level = Level.Debug;
                    break;
                case LogLevel.Info:
                    level = Level.Info;
                    break;
                case LogLevel.Warn:
                    level = Level.Warn;
                    break;
                case LogLevel.Error:
                    level = Level.Error;
                    break;
                case LogLevel.Fatal:
                    level = Level.Fatal;
                    break;
                default:
                    level = Level.Info;
                    break;
            }
            _logger = LoggerManager.GetLogger(System.Reflection.Assembly.GetCallingAssembly(), logLevel.ToString());
            if (_log.IsDebugEnabled)
            {
                _logger.Log(typeof(Logger), level, message, exception);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 输出普通日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void Log(LogLevel level, string format, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _log.DebugFormat(CultureInfo.InvariantCulture, format, args);
                    break;
                case LogLevel.Info:
                    _log.InfoFormat(CultureInfo.InvariantCulture, format, args);
                    break;
                case LogLevel.Warn:
                    _log.WarnFormat(CultureInfo.InvariantCulture, format, args);
                    break;
                case LogLevel.Error:
                    _log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
                    break;
                case LogLevel.Fatal:
                    _log.FatalFormat(CultureInfo.InvariantCulture, format, args);
                    break;
            }
        }

        /// <summary>
        /// 格式化输出异常信息
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private void Log(LogLevel level, string message, Exception exception)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _log.Debug(message, exception);
                    break;
                case LogLevel.Info:
                    _log.Info(message, exception);
                    break;
                case LogLevel.Warn:
                    _log.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _log.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    _log.Fatal(message, exception);
                    break;
            }
        }
        #endregion
    }


    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
