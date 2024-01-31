namespace Infoware.QueryablePagedList
{
    /// <summary>
    /// Provides the interface(s) for paged list of any type.
    /// </summary>
    /// <typeparam name="T">The type for paging.</typeparam>
    public interface IPagedList<T>
    {
        PaginationData PaginationData { get; }
        /// <summary>
        /// Gets the current page items.
        /// </summary>
        IList<T> Items { get; }
    }
}
