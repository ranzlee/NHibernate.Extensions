using System.Collections.Generic;
using System.Reflection;
using Glimpse.Core.Extensibility;

namespace NHibernate.Glimpse.Core
{
    internal class LogStatistic
    {
        internal LogStatistic()
        {
            StackFrames = new List<string>();
        }

        internal string Id { get; set; }

        internal string Sql { get; set; }

        internal TimerResult Point { get; set; }

        internal string ExecutionType { get; set; }

        internal string ExecutionMethod { get; set; }
        
        internal string CommandNotification { get; set; }

        internal string LoadNotification { get; set; }

        internal string ConnectionNotification { get; set; }

        internal string FlushNotification { get; set; }

        internal string TransactionNotification { get; set; }

        internal IList<string> StackFrames { get; set; }
    }
}