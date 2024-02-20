using Task7.Contracts;

namespace Task7.Models
{
    public record RetryResponse(TimeSpan Delay) : IResponse;
}
