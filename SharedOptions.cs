using System.CommandLine;
using System.CommandLine.Invocation;

namespace csharp_days
{
    internal class SharedOptions
    {
        private readonly Option<string[]> categoriesOption = new("--categories", "Filter by the categories of the event (separate with whitespace or comma)")
        {
            AllowMultipleArgumentsPerToken = true
        };

        private readonly Option<string> descriptionOption = new("--description", "Filter by the description of the event");

        private readonly Option<DateOnly> dateOption = new("--date", "Filter by the date of the event");

        private readonly Option<bool> noCategoryOption = new("--no-category", "Filter events with no category");

        private readonly Option<bool> todayOption = new("--today", "Filter events that are happening today");

        private readonly Option<DateOnly> beforeDateOption = new("--before-date", "Filter events that are happening before the specified date");

        private readonly Option<DateOnly> afterDateOption = new("--after-date", "Filter events that are happening after the specified date");

        private readonly Option<bool> excludeOption = new("--exclude", "Exclude the specified categories (use with --categories)");

        private List<Option> options;

        public SharedOptions()
        {
            options = new List<Option>
            {
                categoriesOption,
                descriptionOption,
                dateOption,
                noCategoryOption,
                todayOption,
                beforeDateOption,
                afterDateOption,
                excludeOption
            };
        }

        public List<Option> Options { get => options; }

        public void handler(InvocationContext context, ref List<Event> events)
        {
            var eventManager = EventManager.Instance;
            // Get the values of the options
            string[] categories = context.ParseResult.GetValueForOption(categoriesOption)!;
            string description = context.ParseResult.GetValueForOption(descriptionOption)!;
            DateOnly date = context.ParseResult.GetValueForOption(dateOption);
            bool noCategory = context.ParseResult.GetValueForOption(noCategoryOption);
            bool today = context.ParseResult.GetValueForOption(todayOption);
            DateOnly beforeDate = context.ParseResult.GetValueForOption(beforeDateOption);
            DateOnly afterDate = context.ParseResult.GetValueForOption(afterDateOption);
            bool exclude = context.ParseResult.GetValueForOption(excludeOption);

            if (categories.Length > 0)
            {
                if (categories[0].Contains(','))
                {
                    categories = categories[0].Split(",");
                }
                events = eventManager.FilterByCategories(categories, exclude);
            }

            if (noCategory)
            {
                events = eventManager.FilterByNoCategory();
            }

            if (!string.IsNullOrEmpty(description))
            {
                events = eventManager.FilterByDescription(description);
            }

            if (date != DateOnly.MinValue)
            {
                events = eventManager.FilterByDate(date);
            }

            if (beforeDate != DateOnly.MinValue && afterDate == DateOnly.MinValue)
            {
                events = eventManager.FilterByBeforeDate(beforeDate);
            }

            if (afterDate != DateOnly.MinValue && beforeDate == DateOnly.MinValue)
            {
                events = eventManager.FilterByAfterDate(afterDate);
            }

            if (afterDate != DateOnly.MinValue && beforeDate != DateOnly.MinValue)
            {
                events = eventManager.FilterByDateRange(afterDate, beforeDate);
            }

            if (today)
            {
                events = eventManager.FilterByToday();
            }
        }
    }
}
