using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_days
{
    internal class EventManager
    {
        private static readonly EventManager instance = new EventManager();

        public static EventManager Instance => instance;

        private List<Event> events;

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

        public EventManager()
        {
            events = new List<Event>();
        }

        public void SortEventsByDate()
        {
            events.Sort((e, other) => e.Date.CompareTo(other.Date));
        }
       

    }
}
