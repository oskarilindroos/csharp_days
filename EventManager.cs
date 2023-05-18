using CsvHelper;
using CsvHelper.Configuration;
using NodaTime;
using NodaTime.Extensions;
using System.Globalization;

namespace csharp_days
{
    internal class CsvHeaders
    {
        public DateOnly date { get; set; }
        public string category { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    internal class EventManager
    {
        private static readonly EventManager instance = new();

        public static EventManager Instance => instance;

        private List<Event> events;

        public List<Event> GetEvents() => events;

        private EventManager() => events = new List<Event>();

        public static string? GetEventsPath()
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(userHomeDirectory))
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

        public void AddEvent(Event e) => events.Add(e);

        public void DeleteEvent(Event e) => events.Remove(e);

        public void LoadEvents(string eventsPath)
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                // Allow headers to be in any case
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using StreamReader reader = new(eventsPath);
            using CsvReader csv = new(reader, config);
            while (csv.Read())
            {
                try
                {
                    var record = csv.GetRecord<CsvHeaders>();
                    if (record != null)
                    {
                        events.Add(new Event(record.date.ToLocalDate(), record.category, record.description));
                    }
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
                    // Skip the line with an error and continue reading
                    continue;
                }
            }
        }

        public void SaveEvents(string eventsPath)
        {
            // Use a custom culture to ensure the date is always saved in ISO 8601 format
            CultureInfo customCulture = new("en-US")
            {
                DateTimeFormat =
                {
                    ShortDatePattern = "yyyy-MM-dd",
                }
            };

            using StreamWriter writer = new(eventsPath);
            using CsvWriter csv = new(writer, customCulture);
            csv.WriteHeader<CsvHeaders>();
            foreach (Event e in events)
            {
                csv.NextRecord();
                csv.WriteRecord(new CsvHeaders
                {
                    date = e.Date.ToDateOnly(),
                    category = e.Category,
                    description = e.Description
                });
            }
        }

        public List<Event> FilterByCategories(string[] categories, bool exclude)
        {
            if (exclude)
            {
                return events.Where(e => !categories.Contains(e.Category)).ToList();
            }
            else
            {
                return events.Where(e => categories.Contains(e.Category)).ToList();
            }
        }

        public List<Event> FilterByNoCategory()
        {
            return events.Where(e => string.IsNullOrWhiteSpace(e.Category)).ToList();
        }

        public List<Event> FilterByDescription(string description)
        {
            return events.Where(e => e.Description.StartsWith(description)).ToList();
        }

        public List<Event> FilterByDate(DateOnly date)
        {
            return events.Where(e => e.Date == date.ToLocalDate()).ToList();
        }

        public List<Event> FilterByBeforeDate(DateOnly beforeDate)
        {
            return events.Where(e => e.Date < beforeDate.ToLocalDate()).ToList();
        }

        public List<Event> FilterByAfterDate(DateOnly afterDate)
        {
            return events.Where(e => e.Date > afterDate.ToLocalDate()).ToList();
        }

        public List<Event> FilterByDateRange(DateOnly startDate, DateOnly endDate)
        {
            return events.Where(e => e.Date > startDate.ToLocalDate() || e.Date < endDate.ToLocalDate()).ToList();
        }

        public List<Event> FilterByToday()
        {
            return events.Where(e => e.Date == DateTime.Now.ToLocalDateTime().Date).ToList();
        }

        public void PrintEvents()
        {
            foreach (Event e in events)
            {
                Period difference = Period.Between(e.Date, DateTime.Now.ToLocalDateTime().Date);
                Console.WriteLine($"{e} -- {e.GetDifferenceString(difference)}");
            }
        }

        public void SortEventsByDate()
        {
            events.Sort((e, other) => e.Date.CompareTo(other.Date));
        }


    }
}
