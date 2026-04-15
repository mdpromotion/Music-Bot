namespace Backend.Infrastructure.Interfaces
{
    public interface IProcessRunner
    {
        Task<int> RunAsync(string fileName, string args, CancellationToken ct);
    }
}
