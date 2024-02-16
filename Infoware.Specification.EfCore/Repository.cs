using Ardalis.Specification;
using Infoware.Specification.EfCore.Evaluators;
using Infoware.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infoware.Specification.EfCore
{
	public abstract class Repository<T> : IRepository<T> where T : class, IAggregateRoot
	{
		private readonly DbContext _dbContext;
		private readonly ISpecificationEvaluator _specificationEvaluator;

		public Repository(DbContext dbContext) : this(dbContext, SpecificationEvaluator.Default)
		{
		}

		/// <inheritdoc/>
		public Repository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator)
		{
			_dbContext = dbContext;
			_specificationEvaluator = specificationEvaluator;
		}

		public virtual T Insert(T entity)
		{
			return _dbContext.Set<T>().Add(entity).Entity;
		}

		public virtual void Update(T entity)
		{
			_dbContext.Set<T>().Update(entity);
		}

		public virtual void Delete(T entity)
		{
			_dbContext.Set<T>().Remove(entity);
		}

		/// <inheritdoc/>
		public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
		{
			return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
		}

		public virtual IQueryable<T> Select(ISpecification<T> specification)
		{
			return WithSpec(specification);
		}

		public virtual IQueryable<TResult> Select<TResult>(ISpecification<T, TResult> specification)
		{
			return WithSpec(specification);
		}

		/// <summary>
		/// Filters the entities  of <typeparamref name="T"/>, to those that match the encapsulated query logic of the
		/// <paramref name="specification"/>.
		/// </summary>
		/// <param name="specification">The encapsulated query logic.</param>
		/// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
		public virtual IQueryable<T> WithSpec(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
		{
			return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
		}

		/// <summary>
		/// Filters all entities of <typeparamref name="T" />, that matches the encapsulated query logic of the
		/// <paramref name="specification"/>, from the database.
		/// <para>
		/// Projects each entity into a new form, being <typeparamref name="TResult" />.
		/// </para>
		/// </summary>
		/// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
		/// <param name="specification">The encapsulated query logic.</param>
		/// <returns>The filtered projected entities as an <see cref="IQueryable{T}"/>.</returns>
		public virtual IQueryable<TResult> WithSpec<TResult>(ISpecification<T, TResult> specification)
		{
			return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
		}

		/// <inheritdoc/>
		public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
		{
			var queryResult = await WithSpec(specification).ToListAsync(cancellationToken);

			return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
		}

		/// <inheritdoc/>
		public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
		{
			var queryResult = await WithSpec(specification).ToListAsync(cancellationToken);

			return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
		}
	}
}
