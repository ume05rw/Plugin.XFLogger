﻿using System;

namespace Plugin.XFLogger.Abstractions
{
    /// <summary>
    /// Logger base class
    /// </summary>
    public class LoggerBase : ILogger
    {
        private class Locker
        {
            public bool IsLocked { get; set; }

            public Locker()
            {
                this.IsLocked = false;
            }


            public void LockedInvoke(Action action)
            {
                if (action == null)
                    throw new ArgumentException("Required Argument action.");

                Exception exception = null;
                lock (this)
                {
                    this.IsLocked = true;
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        this.IsLocked = false;
                    }
                }

                if (exception != null)
                    throw exception;
            }
        }


        private string logFileName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:読み取られていないプライベート メンバーを削除", Justification = "<保留中>")]
        private int maxLogFilesCount;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:読み取られていないプライベート メンバーを削除", Justification = "<保留中>")]
        private int maxLogFileSizeKb;
        private LogLevel logLevel;
        private bool logToConsole;
        private LogTimeOption logTimeOption;
        private readonly Locker _locker;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerBase()
        {
            logFileName = "app.log";
            maxLogFilesCount = 3;
            maxLogFileSizeKb = 1000;
            logLevel = LogLevel.Warn;
            logTimeOption = LogTimeOption.DateTimeNow;
            logToConsole = false;
            this._locker = new Locker();
        }

        protected void LockedInvoke(Action action)
            => this._locker.LockedInvoke(action);

        /// <summary>
        /// Get the log file name
        /// </summary>
        /// <returns></returns>
        public string GetLogFileName()
        {
            return this.logFileName;
        }
        /// <summary>
        /// Get log level
        /// </summary>
        /// <returns></returns>
        public LogLevel GetLogLevel()
        {
            return logLevel;
        }
        /// <summary>
        /// Get logToConsole
        /// </summary>
        /// <returns></returns>
        public bool GetLogToConsole()
        {
            return logToConsole;
        }
        /// <summary>
        /// Format message
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public string FormatMessage(
            LogLevel logLevel = LogLevel.Warn,
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            DateTime logTime = logTimeOption == LogTimeOption.DateTimeUtcnow
                ? DateTime.UtcNow
                : DateTime.Now;
            string formattedMessage;
            if (exception == null)
            {
                formattedMessage
                    = String.Format(
                        "{0} {1} {2} {3}",
                        logLevel,
                        logTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        tag,
                        message
                    );
            }
            else
            {
                formattedMessage
                    = String.Format(
                        "{0} {1} {2} {3} EXCEPTION: {4} STACK TRACE: {5}",
                        logLevel.ToString(),
                        logTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        tag,
                        message,
                        exception?.Message,
                        exception?.StackTrace?.ToString()
                    );
            }
            formattedMessage += Environment.NewLine;
            return formattedMessage;
        }
        /// <summary>
        /// Configure logger
        /// </summary>
        /// <param name="logTimeOption">DateTime.Now / DateTime.Utcnow in Log</param>
        /// <param name="logFileName"></param>
        /// <param name="maxLogFilesCount"></param>
        /// <param name="maxLogFileSizeKb"></param>
        /// <param name="logLevel"></param>
        /// <param name="logToConsole"></param>
        public virtual void Configure(
            LogTimeOption logTimeOption = LogTimeOption.DateTimeNow,
            string logFileName = "app.log",
            int maxLogFilesCount = 3,
            int maxLogFileSizeKb = 100,
            LogLevel logLevel = LogLevel.Warn,
            bool logToConsole = false
        )
        {
            this.logTimeOption = logTimeOption;
            this.logFileName = logFileName;
            this.maxLogFilesCount = maxLogFilesCount;
            this.maxLogFileSizeKb = maxLogFileSizeKb;
            this.logLevel = logLevel;
            this.logToConsole = logToConsole;
        }

        /// <summary>
        /// Get local storage path
        /// </summary>
        /// <returns></returns>
        public virtual string GetLocalStoragePath()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Log meggage
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Log(
            LogLevel logLevel = LogLevel.Warn,
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Debug(
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            Log(LogLevel.Debug, tag, message, exception);
        }
        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Info(
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            Log(LogLevel.Info, tag, message, exception);
        }
        /// <summary>
        /// Log Warn
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Warn(
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            Log(LogLevel.Warn, tag, message, exception);
        }
        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Error(
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            Log(LogLevel.Error, tag, message, exception);
        }
        /// <summary>
        /// Log Fatal
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public virtual void Fatal(
            string tag = "tag",
            string message = "message",
            Exception exception = null
        )
        {
            Log(LogLevel.Fatal, tag, message, exception);
        }
        /// <summary>
        /// Get local storage path
        /// </summary>
        /// <returns></returns>
        public virtual string GetAll(bool inDescendingOrder = true)
        {
            string log = "";
            return log;
        }
        /// <summary>
        /// Purge log
        /// </summary>
        public virtual void Purge()
        {
            throw new NotImplementedException();
        }

    }
}
