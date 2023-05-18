using NodaTime.Extensions;
using NodaTime;
using System.CommandLine;

namespace csharp_days
{
    internal class ListCommand : Command
    {
        public ListCommand() : base("list", "list events")
        {
            SharedOptions sharedOptions = new();

            // Add the shared options to this command
            foreach (var option in sharedOptions.Options)
            {
                this.AddOption(option);
            }

            this.SetHandler((context) =>
            {
                var eventManager = EventManager.Instance;
                List<Event> filteredEvents = eventManager.GetEvents();
                sharedOptions.handler(context, ref filteredEvents);

                foreach (var e in filteredEvents)
                {
                    Period difference = Period.Between(e.Date, DateTime.Now.ToLocalDateTime().Date);
                    Console.WriteLine($"{e} -- {e.GetDifferenceString(difference)}");
                }
            });
        }
    }
}
