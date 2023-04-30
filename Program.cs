using System.CommandLine;
using System;
using System.CommandLine;
using System;
using System.CommandLine;
using System;
using System.CommandLine;
using System;
using System.CommandLine;
using System;
using System.CommandLine;

            if (eventsPath == null)
            {
                return 1;
            }

            // Load the events from the csv file
{
   
            RootCommand rootCommand = new();
   
            // Options
            Option<string[]> categoriesOption = new("--categories", "Filter by categories of the event (separate with whitespace or ,)");
            categoriesOption.AllowMultipleArgumentsPerToken = true;

            Option<string> descriptionOption = new("--description", "Filter by the description of the event");

            Option<DateOnly> dateOption = new("--date", "Filter by the date of the event");

            Option<bool> noCategoryOption = new("--no-category", "Filter by events with no category");

            Option<bool> excludeOption = new("--exclude", "Exclude the specified categories (use with --categories)");

            // Subcommands
            Command listCommand = new("list", "List events")
            {
                categoriesOption,
                excludeOption,
                descriptionOption,
                dateOption,
                noCategoryOption
            };

            Command addCommand = new("add", "Add an event");

            Command deleteCommand = new("delete", "Delete an event");

            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);
            rootCommand.AddCommand(addCommand);
            /** Subcommand handlers **/
            // List command handler
            listCommand.SetHandler((categories, description, date, noCategory, exclude) =>
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

                eventManager.PrintEvents();
            }, categoriesOption, descriptionOption, dateOption, noCategoryOption, excludeOption);

            // Add command handler


            // Delete command handler

            rootCommand.AddCommand(deleteCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(deleteCommand);


            eventManager.SortEventsByDate();
            foreach (var e in events)
            {
                Period difference = Period.Between(e.Date, DateTime.Now.ToLocalDateTime().Date);
                Console.WriteLine($"{e} -- {e.getDifferenceString(difference)}");
            }
        }
    }
}