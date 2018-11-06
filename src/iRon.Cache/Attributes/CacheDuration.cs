namespace iRon.Cache.Attributes
{
    using System;
    using iRon.Cache.Enums;

    public class CacheDuration: Attribute
    {
        private readonly Duration duration;
        public CacheDuration(Duration duration)
        {
            this.duration = duration;
        }

        public Duration Duration => this.duration;
    }
}
