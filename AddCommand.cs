using System.CommandLine;
using NodaTime.Extensions;

namespace csharp_days
{
    internal class AddCommand : Command
    {
        private readonly Option<string> categoryOption = new("--category", "The category of the event");

        private readonly Option<string> descriptionOption = new("--description", "The description of the event (required)")
        {
            IsRequired = true
        };

        private readonly Option<DateOnly> dateOption = new("--date", "The date of the event");

        public AddCommand() : base("add", "add an event")
        {
            AddOption(categoryOption);
            AddOption(descriptionOption);
            AddOption(dateOption);

            this.SetHandler((context) =>
            {
                var eventManager = EventManager.Instance;
                string? eventsPath = EventManager.GetEventsPath();
                string description = context.ParseResult.GetValueForOption(descriptionOption)!;
                string category = context.ParseResult.GetValueForOption(categoryOption)!;
                DateOnly date = context.ParseResult.GetValueForOption(dateOption)!;

                if (date == DateOnly.MinValue)
                {
                    date = DateOnly.FromDateTime(DateTime.Now);
                }

                Event newEvent = new(date.ToLocalDate(), category, description);
                eventManager.AddEvent(newEvent);

                eventManager.SortEventsByDate();
                eventManager.SaveEvents(eventsPath!);
                Console.WriteLine($"Event added: {newEvent}");
            });
        }
    }
}
