using System.ComponentModel;
using System.Diagnostics;

namespace scaffoldit.Shared;

// This class is based off of what can be found here: https://github.com/dotnet/aspnetcore/blob/main/src/Shared/Process/ProcessExtensions.cs
internal static class ProcessExtensions
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

    public static void KillTree(this Process process) => process.KillTree(_defaultTimeout);

    public static void KillTree(this Process process, TimeSpan timeout)
    {
        var pid = process.Id;
        if (OperatingSystem.IsWindows())
        {
            RunProcessAndWaitForExit(
                "taskkill",
                $"/T /F /PID {pid}",
                timeout,
                out var _);
        }
        else
        {
            var children = new HashSet<int>();
            GetAllChildIdsUnix(pid, children, timeout);
            foreach (var childId in children)
            {
                KillProcessUnix(childId, timeout);
            }
            KillProcessUnix(pid, timeout);
        }
    }

    private static void GetAllChildIdsUnix(int parentId, ISet<int> children, TimeSpan timeout)
    {
        try
        {
            RunProcessAndWaitForExit(
                "pgrep",
                $"-P {parentId}",
                timeout,
                out var stdout);

            if (!string.IsNullOrEmpty(stdout))
            {
                using (var reader = new StringReader(stdout))
                {
                    while (true)
                    {
                        var text = reader.ReadLine();
                        if (text == null)
                        {
                            return;
                        }

                        if (int.TryParse(text, out var id))
                        {
                            children.Add(id);
                            // Recursively get the children
                            GetAllChildIdsUnix(id, children, timeout);
                        }
                    }
                }
            }
        }
        catch (Win32Exception ex) when (ex.Message.Contains("No such file or directory"))
        {
            // This probably means that pgrep isn't installed. Nothing to be done?
        }
    }

    private static void KillProcessUnix(int processId, TimeSpan timeout)
    {
        try
        {
            RunProcessAndWaitForExit(
                "kill",
                $"-TERM {processId}",
                timeout,
                out var stdout);
        }
        catch (Win32Exception ex) when (ex.Message.Contains("No such file or directory"))
        {
            // This probably means that the process is already dead
        }
    }

    private static void RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        var process = Process.Start(startInfo);

        stdout = null;
        if (process.WaitForExit((int)timeout.TotalMilliseconds))
        {
            stdout = process.StandardOutput.ReadToEnd();
        }
        else
        {
            process.Kill();
        }
    }
}