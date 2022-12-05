using System.CommandLine;
using System.Runtime.CompilerServices;

namespace scaffoldit.Commands;

public class NewProjectCommand : ScaffolditCommand
{
    public NewProjectCommand() : base("new", "Create a new scaffoldit project.")
    {
    }

    public sealed override ScaffolditCommand Build()
    {
        var webApiCommand = new WebApiCommand()
            .Build();
        this.AddCommand(webApiCommand);

        return this;
    }
}