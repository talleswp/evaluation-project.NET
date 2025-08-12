
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository
    /// </summary>
    /// <param name="sale">The sale to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository (logical delete by setting IsCancelled to true)
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was cancelled, false if not found</returns>
    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales with optional pagination, sorting, and filtering
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve</param>
    /// <param name="pageSize">The number of sales per page</param>
    /// <param name="sortBy">The field to sort by</param>
    /// <param name="sortOrder">The sort order (asc or desc)</param>
    /// <param name="customer">Optional customer filter</param>
    /// <param name="branch">Optional branch filter</param>
    /// <param name="isCancelled">Optional cancellation status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of sales</returns>
    Task<IEnumerable<Sale>> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        string sortBy, 
        string sortOrder, 
        string? customer, 
        string? branch, 
        bool? isCancelled,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the total count of sales based on filters.
    /// </summary>
    /// <param name="customer">Optional customer filter</param>
    /// <param name="branch">Optional branch filter</param>
    /// <param name="isCancelled">Optional cancellation status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count of sales</returns>
    Task<int> GetCountAsync(string? customer, string? branch, bool? isCancelled, CancellationToken cancellationToken = default);
}
