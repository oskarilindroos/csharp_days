# Days

Days utility in C#

## Package dependencies

- CsvHelper 30.0.1 (for CSV parsing)
- System.CommandLine 2.0.0-beta4.22272.1 (for command line parsing)
- NodaTime 3.1.9 (for helpful date formats and methods)

## Compile

You need .NET 7.0 SDK to compile this project.

To compile:

    dotnet build


## Run


    dotnet run [command] [options]

## Usage

    Usage:
    csharp_days [command] [options]

    Options:
        --version       Show version information
        -?, -h, --help  Show help and usage information

    Commands:
      list    list events
      add     add an event
      delete  delete events
