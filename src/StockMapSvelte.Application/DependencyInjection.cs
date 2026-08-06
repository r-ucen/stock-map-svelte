using StockMapSvelte.Application.UseCases.TreeMapUseCases.Handlers;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.Facade;
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
        services.AddScoped<IAccountActionFacade, AccountActionFacade>();
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
        services.AddScoped<GetAllStocksQueriedHandler>();
        services.AddScoped<CreateStockHandler>();
        services.AddScoped<CreateStocksHandler>();
        services.AddScoped<DeleteStockHandler>();
        services.AddScoped<EditStockHandler>();
        services.AddScoped<GetStockHandler>();
        services.AddScoped<GetAllStockProfilesHandler>();
        services.AddScoped<GetPossibleToAddStocksHandler>();
        services.AddScoped<DeleteUserHandler>();
        services.AddScoped<GetAllUsersQueriedHandler>();
        services.AddScoped<GetUserHandler>();
        services.AddScoped<SetPortfolioAsDefaultHandler>();
        services.AddScoped<GetDefaultPortfolioIdHandler>();
        services.AddScoped<GetPortfolioByIdHandler>();
        services.AddScoped<GetAccountInfoHandler>();
        services.AddScoped<DeleteAccountHandler>();
        services.AddScoped<SetPasswordHandler>();
        services.AddScoped<RemoveGoogleExternalLoginHandler>();
        services.AddScoped<UpdateRolesHandler>();
        services.AddScoped<ImportPortfolioFromTrading212Handler>();
        services.AddScoped<BanUserHandler>();
        services.AddScoped<UnbanUserHandler>();
        
        return services;
    }
}