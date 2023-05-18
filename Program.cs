using NodaTime.Extensions;
using System.CommandLine;

namespace csharp_days
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var eventManager = EventManager.Instance; // get the singleton instance

            // Try to load the path to the events file
            string? eventsPath = EventManager.GetEventsPath();
            if (eventsPath == null)
            {
                return 1;
            }

            // Load the events from the csv file
            eventManager.LoadEvents(eventsPath);

            ListCommand listCommand = new();
            AddCommand addCommand = new();
            DeleteCommand deleteCommand = new();

            RootCommand rootCommand = new()
            {
                listCommand,
                addCommand,
                deleteCommand
            };

            return await rootCommand.InvokeAsync(args);
        }
    }
}