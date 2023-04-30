using NodaTime;
using System.Text;

namespace csharp_days
{
    internal class Event
    {
        public LocalDate Date { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public Event(LocalDate date, string category, string description)
        {
            Description = description;
            Category = category;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Date.ToDateOnly().ToString("yyyy-MM-dd")}: {Description} ({Category})";
        }

        public string getDifferenceString(Period p)
        {
            StringBuilder sb = new();

            int years = Math.Abs(p.Years);
            int months = Math.Abs(p.Months);
            int days = Math.Abs(p.Days);

            if (days == 0 && months == 0 && years == 0)
            {
                return "today";
            }

            if (years != 0)
            {
                sb.Append($"{years} years ");
            }

            if (months != 0)
            {
                sb.Append($"{months} months ");
            }

            if (days != 0)
            {
                sb.Append($"{days} days ");
            }

            if (p.Days < 0 || p.Years < 0 || p.Months < 0)
            {
                sb.Insert(0, "in ");
            }
            else
            {
                sb.Append("ago");
            }

            return sb.ToString();
        }
    }
}
