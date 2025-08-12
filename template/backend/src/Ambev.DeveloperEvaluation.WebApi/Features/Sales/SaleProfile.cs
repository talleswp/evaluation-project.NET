
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        // CreateSale
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();
        CreateMap<Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleResult, Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale.CreateSaleResponse>();

        // GetSale
        CreateMap<GetSaleRequest, GetSaleQuery>();
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();

        // GetAllSales
        CreateMap<GetAllSalesRequest, GetAllSalesQuery>();
        CreateMap<Ambev.DeveloperEvaluation.Application.Sales.GetAllSales.GetAllSalesItemResult, Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales.GetAllSalesItemResponse>();
        CreateMap<GetAllSalesResult, PaginatedList<GetAllSalesItemResponse>>()
            .ConvertUsing((src, dest, context) => new PaginatedList<GetAllSalesItemResponse>(
                context.Mapper.Map<List<GetAllSalesItemResponse>>(src.Items),
                src.TotalCount,
                src.PageNumber,
                src.PageSize
            ));

        // UpdateSale
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();

        // CancelSale
        CreateMap<CancelSaleRequest, CancelSaleCommand>();
        CreateMap<CancelSaleResult, CancelSaleResponse>();
    }
}
