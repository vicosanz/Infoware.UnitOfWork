﻿namespace Infoware.QueryablePagedList
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        public PaginationData PaginationData { get; init; } = new();

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<T> Items { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            int totalCount;

            if (source is IQueryable<T> querable)
            {
                totalCount = querable.Count();
                Items = querable.Skip((PaginationData.PageIndex - 1) * PaginationData.PageSize).Take(PaginationData.PageSize).ToList();
            }
            else
            {
                totalCount = source.Count();
                Items = source.Skip((PaginationData.PageIndex - 1) * PaginationData.PageSize).Take(PaginationData.PageSize).ToList();
            }
            PaginationData = new()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        internal PagedList() => Items = Array.Empty<T>();
    }


    /// <summary>
    /// Provides the implementation of the <see cref="IPagedList{T}"/> and converter.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        public PaginationData PaginationData { get; init; } = new();

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<TResult> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize)
        {
            int totalCount;

            if (source is IQueryable<TSource> querable)
            {
                totalCount = querable.Count();
                var items = querable.Skip((PaginationData.PageIndex - 1) * PaginationData.PageSize).Take(PaginationData.PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
            else
            {
                totalCount = source.Count();
                var items = source.Skip((PaginationData.PageIndex - 1) * PaginationData.PageSize).Take(PaginationData.PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
            PaginationData = new()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PaginationData = new()
            {
                PageIndex = source.PaginationData.PageIndex,
                PageSize = source.PaginationData.PageSize,
                TotalCount = source.PaginationData.TotalCount
            };

            Items = new List<TResult>(converter(source.Items));
        }
    }
}
