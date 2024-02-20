namespace Task7.Contracts
{
    public interface IHandler
    {
        Task<IApplicationStatus> GetApplicationStatus(string id);
    }
}
