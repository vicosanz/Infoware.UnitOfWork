using Ardalis.Specification;

namespace Infoware.UnitOfWork
{
    public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}