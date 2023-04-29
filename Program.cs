using NodaTime;
using NodaTime.Extensions;

namespace csharp_days
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>
            {
                new Event(new LocalDate(2023, 4, 30) ,"Birthday", "My birthday"),
                new Event(new LocalDate(2023, 4, 29) ,"Birthday", "My birthday"),
                new Event(new LocalDate(2010, 3, 30) ,"Birthday", "My birthday"),
                new Event(new LocalDate(2026, 4, 29) ,"Birthday", "My birthday"),
            };
            
            events.Sort((e, other) => e.Date.CompareTo(other.Date));
            foreach (var e in events)
            {
               Period difference = Period.Between(e.Date, DateTime.Now.ToLocalDateTime().Date);
               Console.WriteLine($"{e} -- {e.getDifferenceString(difference)}");
            }
        }
    }
}