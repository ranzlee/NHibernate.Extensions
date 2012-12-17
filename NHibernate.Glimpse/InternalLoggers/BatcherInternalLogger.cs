using System;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class BatcherInternalLogger : IInternalLogger
    {
        internal delegate void Logging(object sender, LoggingArgs args);
        internal static event Logging OnLogging;

        public void DebugFormat(string format, params object[] args)
        {
            if (OnLogging == null) return;
            if (format == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var onLogging = OnLogging;
            if (onLogging != null)
            {
                onLogging.Invoke(this, new LoggingArgs
                                           {
                                               Message = new LogStatistic(null, null)
                                                             {
                                                                 CommandNotification =
                                                                     string.Format(format.Trim().UppercaseFirst(), args),
                                                             }
                                           });
            }
        }

        public void Debug(object message)
        {
       
        }

        public void Error(object message)
        {

        }

        public void Error(object message, Exception exception)
        {

        }

        public void ErrorFormat(string format, params object[] args)
        {

        }

        public void Fatal(object message)
        {

        }

        public void Fatal(object message, Exception exception)
        {

        }

        public void Debug(object message, Exception exception)
        {

        }

        public void Info(object message)
        {

        }

        public void Info(object message, Exception exception)
        {

        }

        public void InfoFormat(string format, params object[] args)
        {

        }

        public void Warn(object message)
        {

        }

        public void Warn(object message, Exception exception)
        {

        }

        public void WarnFormat(string format, params object[] args)
        {

        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsDebugEnabled
        {
            get { return LoggerFactory.LogRequest(); }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }
    }
}