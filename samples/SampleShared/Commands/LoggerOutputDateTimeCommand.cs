using Microsoft.Extensions.Logging;

namespace SampleShared
{
    public class LoggerOutputDateTimeCommand : ICommand
    {
        private readonly ILogger<LoggerOutputDateTimeCommand> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LoggerOutputDateTimeCommand(ILogger<LoggerOutputDateTimeCommand> logger, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Execute()
        {
            _logger.LogInformation($"Current DateTime is {_dateTimeProvider.GetCurrentDateTime()}");
        }
    }
}
