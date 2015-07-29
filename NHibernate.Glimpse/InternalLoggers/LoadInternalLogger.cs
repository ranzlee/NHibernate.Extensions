using System;
using Glimpse.Core.Extensibility;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class LoadInternalLogger : IInternalLogger, IInspector
    {
        private static IMessageBroker _messageBroker;
        private static Func<RuntimePolicy> _runtime;

        private const string TargetMessage = "done materializing entity";

        public void Debug(object message)
        {
            if (_runtime == null) return;
            if (_runtime.Invoke() == RuntimePolicy.Off) return;
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            if (!message.ToString().ToLower().Trim().StartsWith(TargetMessage)) return;
            if (!LoggerFactory.LogRequest()) return;
            var item = new LogStatistic(null, null)
                           {
                               LoadNotification =
                                   message.ToString().Replace(TargetMessage, string.Empty).Trim().UppercaseFirst()
                           };
            Log(item);
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

        static void Log(LogStatistic logStatistic)
        {
            if (_messageBroker == null) return;
            _messageBroker.Publish(logStatistic);
        }

        public void Setup(IInspectorContext context)
        {
            if (context == null) return;
            _runtime = context.RuntimePolicyStrategy.Safe();
            _messageBroker = context.MessageBroker;
        }
    }
}