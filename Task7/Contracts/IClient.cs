namespace Task7.Contracts
{
    public interface IClient
    {
        Task<IResponse> GetApplicationStatus(string id, CancellationToken cancellationToken);
    }
}
