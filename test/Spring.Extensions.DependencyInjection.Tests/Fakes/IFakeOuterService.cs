using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Extensions.DependencyInjection.Tests.Fakes;

public interface IFakeOuterService
{
    IFakeService SingleService { get; }

    IEnumerable<IFakeMultipleService> MultipleServices { get; }
}