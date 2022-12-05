using System.CommandLine;
using System.Text;
using System.Text.Json;
using scaffoldit.Shared;

namespace scaffoldit.Commands;

public class WebApiCommand : ScaffolditCommand
{
    public WebApiCommand() : base("webapi", "Create a new dotnet webapi.")
    {
    }

    public sealed override ScaffolditCommand Build()
    {
        var nameOption = new Option<string>(
            name: "--name",
            description: "The name for the output being created. If no name is specified, the name of the output directory is used.");
        
        var outputOption = new Option<string>(
            name: "--output",
            description: "Location to place the generated output.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.ErrorMessage = "Argument output is required";
                    return null;
                }

                string output = result.Tokens.Single().Value;
                if (!Directory.Exists(output))
                {
                    result.ErrorMessage = "Output directory does not exist";
                    return null;
                }
                
                
                return output;
            });
        
        var fileOption = new Option<FileInfo>(
            name: "--config-file",
            description: "Path to a json file containing the config for a new webapi.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.ErrorMessage = "Argument config-file is required";
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
        this.AddOption(outputOption);
        this.AddOption(nameOption);
        
        this.SetHandler(HandleCommand, nameOption, outputOption, fileOption);

        return this;
    }

    public async Task<int> HandleCommand(string name, string output, FileInfo configFile)
    {
        var argsBuilder = new StringBuilder("new gsoftwebapi ");

        if (!string.IsNullOrEmpty(name))
        {
            argsBuilder.Append($"--name {name} ");
        }

        argsBuilder.Append($"--output {output} ");

        using (StreamReader r = new StreamReader(configFile.FullName))
        {
            string json = r.ReadToEnd();
            var configArguments = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            foreach (var (key, value) in configArguments)
            {
                argsBuilder.Append($"--{key} {value} ");
            }
        }

        var args = argsBuilder.ToString();
        Console.WriteLine(args);
        
        using var execution = ProcessEx.Run(AppContext.BaseDirectory, "dotnet", args);
        await execution.Exited;
        
        // var result = new ProcessResult(execution);
        //
        // // Because dotnet new automatically restores but silently ignores restore errors, need to handle restore errors explicitly
        // if (errorOnRestoreError && (execution.Output.Contains("Restore failed.") || execution.Error.Contains("Restore failed.")))
        // {
        //     result.ExitCode = -1;
        // }

        return execution.ExitCode;
    }
}