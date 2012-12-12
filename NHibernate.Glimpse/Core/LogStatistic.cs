using System;
using System.Collections.Generic;

namespace NHibernate.Glimpse.Core
{
    internal class LogStatistic
    {
        internal LogStatistic()
        {
            StackFrames = new List<string>();
        }

        internal string Sql { get; set; }

        internal string CommandNotification { get; set; }

        internal string LoadNotification { get; set; }

        internal string ConnectionNotification { get; set; }

        internal string FlushNotification { get; set; }

        internal string TransactionNotification { get; set; }

        internal IList<string> StackFrames { get; set; }
    }
}