using NodaTime;
using NodaTime.Extensions;
using System;
using System.CommandLine;

namespace csharp_days
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var eventManager = EventManager.Instance;

            string? eventsPath = eventManager.getEventsPath();
   
            eventManager.loadEvents(eventsPath);

            var rootCommand = new RootCommand();

            // Subcommands
            var listCommand = new Command("list", "List events");
            var addCommand = new Command("add", "Add an event");
            var deleteCommand = new Command("delete", "Delete an event");

            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);


            return await rootCommand.InvokeAsync(args);
        }
    }
}