using NodaTime;
using NodaTime.Extensions;

namespace csharp_days
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var eventManager = EventManager.Instance;

            string? eventsPath = eventManager.getEventsPath();
            if (eventsPath == null)
            {
                return;
            }

            eventManager.loadEvents(eventsPath);

            var events = eventManager.getEvents();

            var event1 = new Event(new LocalDate(2023, 04, 04), "test", "test");
            events.Add(event1);

            eventManager.saveEvents(eventsPath);

            eventManager.SortEventsByDate();
            foreach (var e in events)
            {
                Period difference = Period.Between(e.Date, DateTime.Now.ToLocalDateTime().Date);
                Console.WriteLine($"{e} -- {e.getDifferenceString(difference)}");
            }
        }
    }
}