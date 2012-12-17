using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glimpse.Core.Extensibility;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class SqlInternalLogger : IInternalLogger
    {
        private static readonly Assembly ThisAssem = typeof(SqlInternalLogger).Assembly;
        private static readonly Assembly NhAssem = typeof(IInternalLogger).Assembly;
        private static readonly Assembly GlimpseAssem = typeof(ITab).Assembly;
        internal delegate void Logging(object sender, LoggingArgs args);
        internal static event Logging OnSqlCommandExecuted;
        internal static event Logging OnLogging;

        public void Debug(object message)
        {
            if (OnLogging == null) return;
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var stackFrames = new System.Diagnostics.StackTrace().GetFrames();
            var methods = new List<MethodBase>();
            if (stackFrames != null)
            {
                foreach (var frame in stackFrames)
                {
                    var meth = frame.GetMethod();
                    var type = meth.DeclaringType;
                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    //this can happen for emitted types
                    if (type != null)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    {
                        var assem = type.Assembly;
                        if (Equals(assem, ThisAssem)) continue;
                        if (Equals(assem, NhAssem)) continue;
                        if (Equals(assem, GlimpseAssem)) continue;    
                    }
                    methods.Add(frame.GetMethod());
                }
            }
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var frames = methods
                .Select(method => string.Format("{0} -> {1}", (method.DeclaringType == null) ? "DYNAMIC" : method.DeclaringType.ToString(), method))
                .ToList();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            var item = new LogStatistic(null, null)
                           {
                               Sql = message.ToString(),
                               StackFrames = frames,
                               ExecutionType = (methods.Count == 0)
                                                   ? null
                                                   : (methods[0].DeclaringType == null)
                                                         ? "Object"
                                                         : methods[0].DeclaringType.Name,
                               ExecutionMethod = (methods.Count == 0) ? null : methods[0].Name,
                           };
            var onSqlCommandExecuted = OnSqlCommandExecuted;
            if (onSqlCommandExecuted != null)
            {
                onSqlCommandExecuted.Invoke(this, new LoggingArgs { Message = item });
            }
            var onLogging = OnLogging;
            if (onLogging != null)
            {
                onLogging.Invoke(this, new LoggingArgs { Message = item });
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