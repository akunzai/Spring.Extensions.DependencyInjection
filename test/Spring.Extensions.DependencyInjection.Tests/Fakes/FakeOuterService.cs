using System.Collections.Generic;

namespace Spring.Extensions.DependencyInjection.Tests.Fakes
{
    public class FakeOuterService : IFakeOuterService
    {
        public FakeOuterService(
            IFakeService singleService,
            IEnumerable<IFakeMultipleService> multipleServices)
        {
            SingleService = singleService;
            MultipleServices = multipleServices;
        }

        public IFakeService SingleService { get; }

        public IEnumerable<IFakeMultipleService> MultipleServices { get; }
    }
}
