using System;
using System.Diagnostics;

namespace SampleShared
{
    public class DummyDateTimeProvider : IDateTimeProvider,IDisposable
    {
        private bool _disposed;

        public DateTime DateTime { get; set; } = new DateTime(2000,1,1);
        
        public DateTime GetCurrentDateTime()
        {
            return DateTime;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine($"{GetType()} Disposed");
                }
                _disposed = true;
            }
        }
    }
}
