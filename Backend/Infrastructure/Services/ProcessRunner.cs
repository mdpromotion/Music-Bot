using Backend.Infrastructure.Interfaces;
using System.Diagnostics;

namespace Backend.Infrastructure.Services
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<int> RunAsync(string fileName, string args, CancellationToken ct)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync(ct);

            var stderr = await errorTask;
            var stdout = await outputTask;

            return process.ExitCode;
        }
    }
}