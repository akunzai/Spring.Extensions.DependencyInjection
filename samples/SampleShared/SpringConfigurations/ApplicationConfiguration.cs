using Spring.Context.Attributes;

namespace SampleShared
{
    [Configuration]
    public class ApplicationConfiguration
    {
        [ObjectDef]
        public virtual ICommand DateTimeCommand()
        {
            return new ConsoleOutputDateTimeCommand(DateTimeProvider());
        }

        [ObjectDef]
        public virtual IDateTimeProvider DateTimeProvider()
        {
            return new SystemDateTimeProvider();
        }
    }
}
