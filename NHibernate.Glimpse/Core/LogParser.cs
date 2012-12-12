using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Glimpse.Core
{
    public class LogParser
    {
        internal RequestDebugInfo Transform(IDictionary context)
        {
            var selects = 0;
            var updates = 0;
            var deletes = 0;
            var inserts = 0;
            var batchCommands = 0;
            var events = (IList<LogStatistic>)context[Plugin.GlimpseSqlStatsKey];
            if (events == null) return null;
            var info = new RequestDebugInfo
                {
                    GlimpseKey = Guid.NewGuid()
                };
            foreach (var loggingEvent in events)
            {
                if (!string.IsNullOrEmpty(loggingEvent.Sql)  && loggingEvent.Sql.Trim() != string.Empty)
                {
                    var detail = loggingEvent.Sql.TrimStart(' ', '\n', '\r');
                    if (detail.StartsWith("select", StringComparison.OrdinalIgnoreCase)) selects++;
                    if (detail.StartsWith("update", StringComparison.OrdinalIgnoreCase)) updates++;
                    if (detail.StartsWith("delete", StringComparison.OrdinalIgnoreCase)) deletes++;
                    if (detail.StartsWith("insert", StringComparison.OrdinalIgnoreCase)) inserts++;
                    if (detail.StartsWith("batch commands:", StringComparison.OrdinalIgnoreCase)) batchCommands++;
                }
            }
            info.Selects = selects;
            info.Inserts = inserts;
            info.Updates = updates;
            info.Deletes = deletes;
            info.Batch = batchCommands;
            return info;
        }
    }
}