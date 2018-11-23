using System;

namespace SampleShared
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}
