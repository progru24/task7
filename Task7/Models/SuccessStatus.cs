using Task7.Contracts;

namespace Task7.Models
{
    public record SuccessStatus(string ApplicationId, string Status) : IApplicationStatus;
}
