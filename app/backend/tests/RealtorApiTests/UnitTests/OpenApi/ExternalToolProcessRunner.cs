using System.Diagnostics;
using System.Text;

namespace RealtorApiTests.UnitTests.OpenApi;

public sealed record ExternalToolProcessResult(int ExitCode, string StandardOutput, string StandardError);

public static class ExternalToolProcessRunner
{
    public static async Task<ExternalToolProcessResult> RunAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(arguments);
        ArgumentException.ThrowIfNullOrWhiteSpace(workingDirectory);

        var resolvedFileName = ResolveCommandPath(fileName);
        var startInfo = new ProcessStartInfo
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        if (OperatingSystem.IsWindows() && ShouldUseShellOnWindows(resolvedFileName))
        {
            startInfo.FileName = "cmd.exe";
            startInfo.ArgumentList.Add("/d");
            startInfo.ArgumentList.Add("/s");
            startInfo.ArgumentList.Add("/c");
            var command = resolvedFileName.Contains(' ')
                ? $"\"{resolvedFileName}\" {arguments}"
                : $"{resolvedFileName} {arguments}";

            startInfo.ArgumentList.Add(command);
        }
        else
        {
            startInfo.FileName = resolvedFileName;
            startInfo.Arguments = arguments;
        }

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException($"Unable to start '{fileName}'.");
        }

        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);
        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;

        var result = new ExternalToolProcessResult(process.ExitCode, standardOutput, standardError);
        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Command '{fileName} {arguments}' failed with exit code {result.ExitCode}.{Environment.NewLine}" +
                $"STDOUT:{Environment.NewLine}{result.StandardOutput}{Environment.NewLine}" +
                $"STDERR:{Environment.NewLine}{result.StandardError}");
        }

        return result;
    }

    private static string ResolveCommandPath(string fileName)
    {
        if (Path.IsPathRooted(fileName) || Path.HasExtension(fileName) || !OperatingSystem.IsWindows())
        {
            return fileName;
        }

        try
        {
            foreach (var candidate in new[] { $"{fileName}.exe", $"{fileName}.cmd", $"{fileName}.bat", fileName })
            {
                var startInfo = new ProcessStartInfo("where.exe", candidate)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process is null)
                {
                    continue;
                }

                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var resolvedPath = output
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(resolvedPath))
                {
                    return resolvedPath.Trim();
                }
            }

            return fileName;
        }
        catch
        {
            return fileName;
        }
    }

    private static bool ShouldUseShellOnWindows(string fileName)
        => !fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
}
