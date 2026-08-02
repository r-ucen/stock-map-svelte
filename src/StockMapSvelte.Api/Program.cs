using Altairis.Services.Cloudflare;
using Scalar.AspNetCore;
using StockMapSvelte.Api;
using StockMapSvelte.Api.Controllers;
using StockMapSvelte.Api.Middleware;
using StockMapSvelte.Application;
using StockMapSvelte.Infrastructure;
using StockMapSvelte.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(30);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseCloudflare();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigins");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>().RequireRateLimiting("IdentityPolicy");

app.MapPortfolioStreamEndpoints();
app.MapControllers();

app.Run();

public partial class Program { }