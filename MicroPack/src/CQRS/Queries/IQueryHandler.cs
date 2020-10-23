using System.Threading.Tasks;

namespace MicroPack.CQRS.Queries
{
    public interface IQueryHandler<in TQuery,TResult> where TQuery : class, IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}