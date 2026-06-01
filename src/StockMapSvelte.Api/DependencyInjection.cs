using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Api.Services;
using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserContext, UserContext>();
        
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins(allowedOrigins!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        services.AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
            .Configure<IHostEnvironment>((options, env) =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        
                if (!env.IsDevelopment())
                {
                    options.Cookie.Domain = configuration["CookieDomain"];
                }
                
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
        
        services.AddRateLimiter(options =>
        {
            options.AddPolicy("IdentityPolicy", httpContext =>
                {
                    if (httpContext.Request.Method == HttpMethods.Options)
                    {
                        return RateLimitPartition.GetNoLimiter("preflight");
                    }
                    
                    var path = httpContext.Request.Path.Value;

                    if (path.Contains("/manage/info"))
                    {
                        return RateLimitPartition.GetTokenBucketLimiter(
                            partitionKey: httpContext.User.Identity?.Name +
                                          path +
                                          httpContext.Connection.RemoteIpAddress,
                            factory: _ => new TokenBucketRateLimiterOptions()
                            {
                                TokenLimit = 1000,
                                TokensPerPeriod = 100,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            }
                        );
                    }
                    
                    if (path.Contains("/resendConfirmationEmail") || path.Contains("/forgotPassword"))
                    {
                        return RateLimitPartition.GetTokenBucketLimiter(
                            partitionKey: httpContext.User.Identity?.Name +
                                          path +
                                          httpContext.Connection.RemoteIpAddress,
                            factory: _ => new TokenBucketRateLimiterOptions()
                            {
                                TokenLimit = 20,
                                TokensPerPeriod = 1,
                                ReplenishmentPeriod = TimeSpan.FromHours(1),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            }
                        );
                    }

                    if (path.Contains("/register"))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            path +
                            httpContext.Connection.RemoteIpAddress,_ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        });
                    }
                    
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 120,
                            Window = TimeSpan.FromMinutes(1)
                        }
                    );
                }
            );
            
            options.AddPolicy("RoleCheckPolicy", httpContext =>
                {
                    if (httpContext.Request.Method == HttpMethods.Options)
                    {
                        return RateLimitPartition.GetNoLimiter("preflight");
                    }

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: httpContext.User.Identity?.Name ??
                                      httpContext.Connection.RemoteIpAddress?.ToString() ??
                                      "unknown",
                        factory: _ => new TokenBucketRateLimiterOptions()
                        {
                            TokenLimit = 1000,
                            TokensPerPeriod = 100,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }
                    );
                }
            );

            options.AddPolicy("DataPolicy", httpContext =>
                {
                    if (httpContext.Request.Method == HttpMethods.Options)
                    {
                        return RateLimitPartition.GetNoLimiter("preflight");
                    }

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: httpContext.User.Identity?.Name ??
                                      httpContext.Connection.RemoteIpAddress?.ToString() ??
                                      "unknown",
                        factory: _ => new TokenBucketRateLimiterOptions()
                        {
                            TokenLimit = 150,
                            TokensPerPeriod = 20,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }
                    );
                }
            );

            options.AddPolicy("TreeMapDataPolicy", httpContext =>
                {
                    if (httpContext.Request.Method == HttpMethods.Options)
                    {
                        return RateLimitPartition.GetNoLimiter("preflight");
                    }

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: httpContext.User.Identity?.Name ??
                                      httpContext.Connection.RemoteIpAddress?.ToString() ??
                                      "unknown",
                        factory: _ => new TokenBucketRateLimiterOptions()
                        {
                            TokenLimit = 100,
                            TokensPerPeriod = 1,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(12),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }
                    );
                }
            );
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(new { 
                    message = "Rate limit exceeded." 
                }, cancellationToken);
            };
        });
        
        return services;
    }
}