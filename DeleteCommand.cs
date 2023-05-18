using System.CommandLine;

namespace csharp_days
{
    internal class DeleteCommand : Command
    {
        Option<bool> dryRunOption = new("--dry-run", "Show which events would be deleted without deleting them");

        Option<bool> deleteAllOption = new("--all", "Delete all events");

        public DeleteCommand() : base("delete", "delete events")
        {
            SharedOptions sharedOptions = new();

            // Add the shared options to this command
            foreach (var option in sharedOptions.Options)
            {
                this.AddOption(option);
            }

            this.AddOption(dryRunOption);
            this.AddOption(deleteAllOption);

            this.SetHandler((context) =>
            {
                var eventManager = EventManager.Instance;
                string eventsPath = EventManager.GetEventsPath()!;
                bool dryRun = context.ParseResult.GetValueForOption(dryRunOption);
                bool deleteAll = context.ParseResult.GetValueForOption(deleteAllOption);

                List<Event> eventsToDelete = new();
                sharedOptions.handler(context, ref eventsToDelete);

                if (dryRun)
                {
                    Console.WriteLine($"Would delete {eventsToDelete.Count} events:");
                    foreach (var e in eventsToDelete)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    if (deleteAll)
                    {
                        eventsToDelete = eventManager.GetEvents().ToList();
                    }

                    // Delete the events
                    foreach (var e in eventsToDelete)
                    {
                        eventManager.DeleteEvent(e);
                    }

                    eventManager.SaveEvents(eventsPath);
                    Console.WriteLine($"Deleted {eventsToDelete.Count} events.");
                };
            });
        }
    }
}
