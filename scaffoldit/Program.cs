using System.CommandLine;
using scaffoldit.Commands;

namespace scaffoldit;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddScaffoldItCommand<NewCommand>();

        return await rootCommand.InvokeAsync(args);
    }
}