using System;

namespace SampleShared
{
    public class DummySystemClock : ISystemClock
    {
        public DateTime Now { get; set; } = new DateTime(2000, 1, 1);
    }
}
