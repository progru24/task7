using Task7.Contracts;

namespace Task7.Models
{
    public record FailureStatus(DateTime? LastRequestTime, int RetriesCount) : IApplicationStatus;
}
