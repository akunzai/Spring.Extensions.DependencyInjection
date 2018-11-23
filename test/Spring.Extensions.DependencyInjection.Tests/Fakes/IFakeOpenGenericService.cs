namespace Spring.Extensions.DependencyInjection.Tests.Fakes
{
    public interface IFakeOpenGenericService<TValue>
    {
        TValue Value { get; }
    }
}
