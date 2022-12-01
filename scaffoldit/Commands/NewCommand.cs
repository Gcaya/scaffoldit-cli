using System.CommandLine;
using System.Runtime.CompilerServices;

namespace scaffoldit.Commands;

public class NewCommand : ScaffolditCommand
{
    public NewCommand() : base("new", "Create a new scaffoldit project.")
    {
        this.Init();
    }

    protected sealed override void Init()
    {
        var webApiCommand = new WebApiCommand();
        this.AddCommand(webApiCommand);
    }
}