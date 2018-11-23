using System;

namespace SampleShared
{
    public class ConsoleOutputDateTimeCommand : ICommand
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConsoleOutputDateTimeCommand(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void Execute()
        {
            Console.WriteLine($"Current DateTime is {_dateTimeProvider.GetCurrentDateTime()}");
        }
    }
}
