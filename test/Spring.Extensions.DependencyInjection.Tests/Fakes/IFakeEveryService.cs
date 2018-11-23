namespace Spring.Extensions.DependencyInjection.Tests.Fakes
{
    public interface IFakeEveryService :
        IFakeService,
        IFakeMultipleService,
        IFakeScopedService,
        IFakeServiceInstance,
        IFakeSingletonService,
        IFakeOpenGenericService<PocoClass>
    {
    }
}
