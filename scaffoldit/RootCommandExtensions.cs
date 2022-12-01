using System.CommandLine;
using scaffoldit.Commands;

namespace scaffoldit;

public static class RootCommandExtensions
{
    public static void AddScaffoldItCommand<TCommand>(this RootCommand rootCommand) 
        where TCommand: ScaffolditCommand, new()
    {
        var command = new TCommand();
        rootCommand.AddCommand(command);
    }
}