namespace Spring.Extensions.DependencyInjection.Tests.Fakes;

public interface IFactoryService
{
    IFakeService FakeService { get; }

    int Value { get; }
}