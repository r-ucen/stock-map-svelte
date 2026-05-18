using StockMapBlazor.Application.UseCases.TreeMapUseCases.Handlers;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.Facade;
using StockMapSvelte.Application.Services.TreeMap;
using StockMapSvelte.Application.Services.TreeMap.Coloring;
using StockMapSvelte.Application.Services.TreeMap.Formatting;
using StockMapSvelte.Application.Services.TreeMap.Layout;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;
using StockMapSvelte.Application.UseCases.StockProfileUseCases.Handlers;
using StockMapSvelte.Application.UseCases.StockUseCases.Handlers;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;
using StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

namespace StockMapSvelte.Application;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITreeMapService, TreeMapService>();
        services.AddScoped<ColorScaleService>();
        services.AddScoped<TreemapLayoutCalculator>();
        services.AddScoped<CellDescriptionFormatter>();

        services.AddScoped<IPortfolioFacade, PortfolioFacade>();
        services.AddScoped<ITreeMapFacade, TreeMapFacade>();
        services.AddScoped<IStockFacade, StockFacade>();
        services.AddScoped<IStockProfileFacade, StockProfileFacade>();
        services.AddScoped<IUserFacade, UserFacade>();
        services.AddScoped<IUserSettingFacade, UserSettingFacade>();
        
        services.AddScoped<CreatePortfolioHandler>();
        services.AddScoped<DeletePortfolioHandler>();
        services.AddScoped<EditPortfolioHandler>();
        services.AddScoped<GetPortfoliosByUserIdHandler>();
        services.AddScoped<GetAllPortfoliosHandler>();
        services.AddScoped<GetTreemapDataHandler>();
        services.AddScoped<GetAllStocksHandler>();
        services.AddScoped<CreateStockHandler>();
        services.AddScoped<DeleteStockHandler>();
        services.AddScoped<EditStockHandler>();
        services.AddScoped<GetStockHandler>();
        services.AddScoped<GetAllStockProfilesHandler>();
        services.AddScoped<GetPossibleToAddStocksHandler>();
        services.AddScoped<DeleteUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetUserHandler>();
        services.AddScoped<SetPortfolioAsDefaultHandler>();
        services.AddScoped<GetDefaultPortfolioIdHandler>();
        services.AddScoped<SetToastAutoHideValueHandler>();
        services.AddScoped<GetToastAutoHideValueHandler>();
        services.AddScoped<GetToastAutoHideDelayMsHandler>();
        services.AddScoped<SetToastAutoHideDelayMsHandler>();
        services.AddScoped<GetPortfolioByIdHandler>();
        services.AddScoped<GetAccountInfoHandler>();
        services.AddScoped<DeleteAccountHandler>();
        services.AddScoped<RemoveGoogleExternalLoginHandler>();
        
        return services;
    }
}