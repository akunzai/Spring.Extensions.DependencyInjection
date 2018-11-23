using System;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;

namespace Spring.Extensions.DependencyInjection.Internal
{
    internal class SpringServiceScope : IServiceScope
    {
        private readonly SpringServiceProvider _serviceProvider;
        private bool _disposed;

        public SpringServiceScope(IApplicationContext context, SpringServiceProviderOptions options)
        {
            _serviceProvider = new SpringServiceProvider(context, options);
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _serviceProvider.Dispose();
                }
                _disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
