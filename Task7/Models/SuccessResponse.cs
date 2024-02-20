using Task7.Contracts;

namespace Task7.Models
{
    public record SuccessResponse(string Id, string Status) : IResponse;
}
