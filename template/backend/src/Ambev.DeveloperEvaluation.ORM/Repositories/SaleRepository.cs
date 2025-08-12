
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        sale.CancelSale(); // Lógica de domínio para cancelar
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        string sortOrder,
        string? customer,
        string? branch,
        bool? isCancelled,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = _context.Sales.Include(s => s.Items);

        if (!string.IsNullOrWhiteSpace(customer))
        {
            query = query.Where(s => s.Customer.Contains(customer));
        }

        if (!string.IsNullOrWhiteSpace(branch))
        {
            query = query.Where(s => s.Branch.Contains(branch));
        }

        if (isCancelled.HasValue)
        {
            query = query.Where(s => s.IsCancelled == isCancelled.Value);
        }

        // Sorting
        Expression<Func<Sale, object>> keySelector = sortBy.ToLowerInvariant() switch
        {
            "saledate" => sale => sale.SaleDate,
            "customer" => sale => sale.Customer,
            "totalamount" => sale => sale.TotalAmount,
            "branch" => sale => sale.Branch,
            _ => sale => sale.SaleDate // Default sort
        };

        query = sortOrder.ToLowerInvariant() == "desc" 
            ? query.OrderByDescending(keySelector) 
            : query.OrderBy(keySelector);

        // Pagination
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(string? customer, string? branch, bool? isCancelled, CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = _context.Sales;

        if (!string.IsNullOrWhiteSpace(customer))
        {
            query = query.Where(s => s.Customer.Contains(customer));
        }

        if (!string.IsNullOrWhiteSpace(branch))
        {
            query = query.Where(s => s.Branch.Contains(branch));
        }

        if (isCancelled.HasValue)
        {
            query = query.Where(s => s.IsCancelled == isCancelled.Value);
        }

        return await query.CountAsync(cancellationToken);
    }
}
