using Spring.Context.Attributes;

namespace SampleShared
{
    [Configuration]
    public class SpringConfiguration
    {
        [ObjectDef]
        public virtual ISystemClock SystemClock()
        {
            return new DefaultSystemClock();
        }
    }
}
