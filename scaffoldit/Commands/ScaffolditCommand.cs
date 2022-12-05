using System.CommandLine;

namespace scaffoldit.Commands;

public abstract class ScaffolditCommand : Command
{
    protected ScaffolditCommand(string name, string? description = null) : base(name, description)
    {
    }

    public abstract ScaffolditCommand Build();
}