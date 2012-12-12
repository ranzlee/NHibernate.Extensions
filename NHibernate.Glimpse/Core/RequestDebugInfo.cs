using System;

namespace NHibernate.Glimpse.Core
{
    internal class RequestDebugInfo
    {
        public Guid GlimpseKey { get; set; }

        public int Selects { get; set; }
        
        public int Inserts { get; set; }
        
        public int Updates { get; set; }
        
        public int Deletes { get; set; }
        
        public int Batch { get; set; }
    }
}