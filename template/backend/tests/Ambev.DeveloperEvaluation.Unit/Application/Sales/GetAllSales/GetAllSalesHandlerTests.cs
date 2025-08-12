
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetAllSales;

public class GetAllSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetAllSalesHandler _handler;

    public GetAllSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Should return paginated list of sales")]
    public async Task Should_ReturnPaginatedListOfSales()
    {
        // Arrange
        var query = new GetAllSalesQuery { PageNumber = 1, PageSize = 10, SortBy = "SaleDate", SortOrder = "desc" };
        var sales = new List<Sale> { new Sale("Customer 1", "Branch A"), new Sale("Customer 2", "Branch B") };
        var totalCount = 2;

        _saleRepository.GetCountAsync(query.Customer, query.Branch, query.IsCancelled, Arg.Any<CancellationToken>()).Returns(totalCount);
        _saleRepository.GetAllAsync(
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortOrder,
            query.Customer,
            query.Branch,
            query.IsCancelled,
            Arg.Any<CancellationToken>()
        ).Returns(sales);
        _mapper.Map<IEnumerable<GetAllSalesItemResult>>(sales).Returns(sales.Select(s => new GetAllSalesItemResult { Id = s.Id, Customer = s.Customer }));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(sales.Count);
        result.TotalCount.Should().Be(totalCount);
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalPages.Should().Be(1);
        await _saleRepository.Received(1).GetCountAsync(query.Customer, query.Branch, query.IsCancelled, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).GetAllAsync(
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortOrder,
            query.Customer,
            query.Branch,
            query.IsCancelled,
            Arg.Any<CancellationToken>()
        );
    }
}
