﻿namespace Infoware.QueryablePagedList
{
    public record PaginationData
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; init; } = 1;
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; init; } = 20;
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; init; } = 0;
        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
