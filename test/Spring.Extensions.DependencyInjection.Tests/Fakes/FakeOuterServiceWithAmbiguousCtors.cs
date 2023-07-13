using System;
using System.Collections.Generic;

namespace Spring.Extensions.DependencyInjection.Tests.Fakes;

public class FakeOuterServiceWithAmbiguousCtors : IFakeOuterService
{
    public FakeOuterServiceWithAmbiguousCtors() : this(Array.Empty<IFakeMultipleService>(), null)
    {
    }

    public FakeOuterServiceWithAmbiguousCtors(IFakeService singleService) : this(Array.Empty<IFakeMultipleService>(), singleService)
    {
    }

    public FakeOuterServiceWithAmbiguousCtors(IEnumerable<IFakeMultipleService> multipleServices, IFakeService singleService)
    {
        MultipleServices = multipleServices;
        SingleService = singleService;
    }

    public IFakeService SingleService { get; }

    public IEnumerable<IFakeMultipleService> MultipleServices { get; }
}