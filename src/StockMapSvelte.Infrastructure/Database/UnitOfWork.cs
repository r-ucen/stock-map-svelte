using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Repositories;

namespace StockMapSvelte.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private IPortfolioRepository? _portfolios;
    private IStockProfileRepository? _stockProfiles;
    private IStockRepository? _stocks;
    private IUserSettingRepository? _userSettings;
    
    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public IPortfolioRepository Portfolios =>
        _portfolios ??= new PortfolioRepository(_context);
    
    public IStockProfileRepository StockProfiles =>
        _stockProfiles ??= new StockProfileRepository(_context);
    
    public IStockRepository Stocks =>
        _stocks ??= new StockRepository(_context);

    public IUserSettingRepository UserSettings =>
        _userSettings ??= new UserSettingRepository(_context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync()
    {
        await _context.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}