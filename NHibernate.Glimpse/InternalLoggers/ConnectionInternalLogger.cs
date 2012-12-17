using System;
using System.Globalization;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class ConnectionInternalLogger : IInternalLogger
    {
        internal delegate void Logging(object sender, LoggingArgs args);
        internal static event Logging OnLogging;

        public void Debug(object message)
        {
            if (OnLogging == null) return;
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var timestamp = DateTime.Now;
            var onLogging = OnLogging;
            if (onLogging != null)
            {
                onLogging.Invoke(this, new LoggingArgs
                {
                    Message = new LogStatistic(null, null)
                    {
                        ConnectionNotification =
                              string.Format("{0}{1}", message.ToString().Trim().UppercaseFirst(),
                                            string.Format(" @ {0}.{1}.{2}.{3}",
                                                          timestamp.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                                          timestamp.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                                          timestamp.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                                          timestamp.Millisecond.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0')))
                    }
                });
            }
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

        public void DebugFormat(string format, params object[] args)
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