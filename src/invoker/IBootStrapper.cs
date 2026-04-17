namespace invoker
{
    using System.Threading.Tasks;

    public interface IBootStrapper
    {
        Task Start(CancellationToken cancellationToken);
    }
}
