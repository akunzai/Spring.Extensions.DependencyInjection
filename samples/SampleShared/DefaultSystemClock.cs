using System;

namespace SampleShared
{
    public class DefaultSystemClock : ISystemClock
    {
        public DateTime Now => DateTime.Now;
    }
}
