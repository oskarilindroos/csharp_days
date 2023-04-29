using CsvHelper;
using CsvHelper.Configuration;
using NodaTime.Extensions;
using System.Globalization;

namespace csharp_days
{
    internal class CsvHeaders
    {
        public DateOnly date { get; set; }
        public string category { get; set; }
        public string description { get; set; }
    }

    internal class EventManager
    {
        private static readonly EventManager instance = new EventManager();

        public static EventManager Instance => instance;

        private List<Event> events;

        public List<Event> getEvents()
        {
            return events;
        }

        public EventManager()
        {
            events = new List<Event>();
        }

        public string? getEventsPath()
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrWhiteSpace(userHomeDirectory))
            {
                Console.Error.WriteLine("Unable to determine user home directory");
                return null;
            }

            string daysPath = Path.Combine(userHomeDirectory, ".days");
            if (!Directory.Exists(daysPath))
            {
                Console.Error.WriteLine(daysPath + " directory does not exist, please create it");
                return null;
            }

            string eventsPath = Path.Combine(daysPath, "events.csv");
            if (!File.Exists(eventsPath))
            {
                Console.Error.WriteLine(eventsPath + " file not found");
                return null;
            }

            return eventsPath;
        }

        public void loadEvents(string eventsPath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using (var reader = new StreamReader(eventsPath))
            using (var csv = new CsvReader(reader, config))
            {

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    try
                    {
                        var record = csv.GetRecord<CsvHeaders>()!;
                        events.Add(new Event(record.date.ToLocalDate(), record.category, record.description));
                    }
                    catch (ReaderException re)
                    {
                        Console.Error.Write($"Invalid data on line {re.Context.Parser.Row}: ");
                        if (re.InnerException != null)
                        {
                            Console.Error.WriteLine(re.InnerException.Message);
                        }
                        else
                        {
                            Console.Error.WriteLine(re.Context.Parser.RawRecord);
                        }
                        continue;
                    }
                }
            }
        }

        public void SortEventsByDate()
        {
            events.Sort((e, other) => e.Date.CompareTo(other.Date));
        }


    }
}
