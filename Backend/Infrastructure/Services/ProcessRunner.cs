using Backend.Infrastructure.Interfaces;
using System.Diagnostics;

namespace Backend.Infrastructure.Services
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<int> RunAsync(string fileName, string args, CancellationToken ct)
        {
            var process = new Process
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
            await process.WaitForExitAsync(ct);

            return process.ExitCode;
        }
    }
}
