using System;

namespace SampleShared
{
    public class DummySystemClock : ISystemClock
    {
        public DummySystemClock(DateTime now)
        {
            Now = now;
        }

        public DateTime Now { get; }
    }
}
