using System;
using System.Collections.Generic;
using System.Text;

namespace iRon.Cache
{
    public interface ICacheConfig {
        DurationConfig Duration { get; set; }
        string Prefix { get; set; }
        string ConnectionString { get; set; }
    }
    public class CacheConfig: ICacheConfig
    {
        public string ConnectionString { get; set; }
        public string Prefix { get; set; }
        public DurationConfig Duration { get; set; } = new DurationConfig();
    }

    public class DurationConfig
    {
        public int None { get; set; } = 0;
        public int Low { get; set; } = 30;
        public int Normal { get; set; } = 300;
        public int High { get; set; } = 3600;
        public int Forever { get; set; } = 86400;
    }



}
