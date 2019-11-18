using System;

namespace SampleShared
{
    public interface ISystemClock
    {
        DateTime Now { get; }
    }
}
