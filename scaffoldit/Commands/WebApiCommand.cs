using System.CommandLine;

namespace scaffoldit.Commands;

public class WebApiCommand : ScaffolditCommand
{
    public WebApiCommand() : base("webapi", "Create a new dotnet webapi.")
    {
        this.Init();
    }

    protected sealed override void Init()
    {
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "An option whose argument is parsed as a FileInfo",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.ErrorMessage = "File is required";
                    return null;
                }
                
                string filePath = result.Tokens.Single().Value;
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exist";
                    return null;
                }
                
                return new FileInfo(filePath);
            });
        
        this.AddOption(fileOption);
        
        this.SetHandler(HandleWebApi, fileOption);
    }

    private void HandleWebApi(FileInfo? fileInfo)
    {
        Console.WriteLine("Sup dawg");
    }
}