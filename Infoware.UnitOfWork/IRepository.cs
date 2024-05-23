using Ardalis.Specification;

namespace Infoware.UnitOfWork
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        void Delete(T entity);
        Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        T Insert(T entity);
        Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
        IQueryable<T> Select(ISpecification<T> specification);
        IQueryable<TResult> Select<TResult>(ISpecification<T, TResult> specification);
        void Update(T entity);
        IQueryable<T> WithSpec(ISpecification<T> specification, bool evaluateCriteriaOnly = false);
        IQueryable<TResult> WithSpec<TResult>(ISpecification<T, TResult> specification);
    }
}