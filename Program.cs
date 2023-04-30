using NodaTime.Extensions;
using System.CommandLine;

namespace csharp_days
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var eventManager = EventManager.Instance; // get the singleton instance

            // Try to load the path to the events file
            string? eventsPath = EventManager.GetEventsPath();
            if (eventsPath == null)
            {
                return 1;
            }

            // Load the events from the csv file
            eventManager.LoadEvents(eventsPath);

            RootCommand rootCommand = new();

            /** Options **/
            // Add command specific options
            Option<string> add_categoryOption = new("--category", "The category of the event");
            Option<string> add_descriptionOption = new("--description", "The description of the event (required)")
            {
                IsRequired = true
            };
            Option<DateOnly> add_dateOption = new("--date", "The date of the event");

            // Shared options
            Option<string[]> shared_categoriesOption = new("--categories", "Filter by the categories of the event (separate with whitespace or ,)")
            {
                AllowMultipleArgumentsPerToken = true
            };
            Option<string> shared_descriptionOption = new("--description", "Filter by the description of the event");
            Option<DateOnly> shared_dateOption = new("--date", "Filter by the date of the event");
            Option<bool> shared_noCategoryOption = new("--no-category", "Filter events with no category");
            Option<bool> shared_todayOption = new("--today", "Filter events that are happening today");
            Option<DateOnly> shared_beforeDateOption = new("--before-date", "Filter events that are happening before the specified date");
            Option<DateOnly> shared_afterDateOption = new("--after-date", "Filter events that are happening after the specified date");
            Option<bool> shared_excludeOption = new("--exclude", "Exclude the specified categories (use with --categories)");

            //** Subcommands **/
            Command listCommand = new("list", "List events")
            {
                shared_categoriesOption,
                shared_excludeOption,
                shared_descriptionOption,
                shared_dateOption,
                shared_noCategoryOption,
                shared_todayOption,
                shared_beforeDateOption,
                shared_afterDateOption,
            };

            Command addCommand = new("add", "Add an event")
            {
                add_descriptionOption,
                add_dateOption,
                add_categoryOption
            };

            Command deleteCommand = new("delete", "Delete events");

            rootCommand.Add(listCommand);
            rootCommand.Add(addCommand);
            rootCommand.Add(deleteCommand);

            /** Subcommand handlers **/
            // List command handler
            listCommand.SetHandler((categories, description, date, beforeDate, afterDate, today, noCategory, exclude) =>
            {
                if (categories.Length > 0)
                {
                    if (categories[0].Contains(','))
                    {
                        categories = categories[0].Split(",");
                    }
                    eventManager.FilterByCategories(categories, exclude);
                }

                if (noCategory)
                {
                    eventManager.FilterByNoCategory();
                }

                if (!string.IsNullOrEmpty(description))
                {
                    eventManager.FilterByDescription(description);
                }

                if (date != DateOnly.MinValue)
                {
                    eventManager.FilterByDate(date);
                }

                if (beforeDate != DateOnly.MinValue && afterDate == DateOnly.MinValue)
                {
                    eventManager.FilterByBeforeDate(beforeDate);
                }

                if (afterDate != DateOnly.MinValue && beforeDate == DateOnly.MinValue)
                {
                    eventManager.FilterByAfterDate(afterDate);
                }

                if (afterDate != DateOnly.MinValue && beforeDate != DateOnly.MinValue)
                {
                    eventManager.FilterByDateRange(afterDate, beforeDate);
                }

                if (today)
                {
                    eventManager.FilterByToday();
                }

                eventManager.PrintEvents();
            }, shared_categoriesOption, shared_descriptionOption, shared_dateOption, shared_beforeDateOption, shared_afterDateOption, shared_todayOption, shared_noCategoryOption, shared_excludeOption);

            // Add command handler
            addCommand.SetHandler((date, category, description) =>
            {
                if (date == DateOnly.MinValue)
                {
                    date = DateOnly.FromDateTime(DateTime.Now);
                }

                Event newEvent = new(date.ToLocalDate(), category, description);
                eventManager.AddEvent(newEvent);

                eventManager.SortEventsByDate();
                eventManager.SaveEvents(eventsPath);
                Console.WriteLine($"Event added: {newEvent}");
            }, add_dateOption, add_categoryOption, add_descriptionOption);


            // Delete command handler


            return await rootCommand.InvokeAsync(args);
        }
    }
}