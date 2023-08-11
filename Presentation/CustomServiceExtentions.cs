using Application.Repositories;
using Application.UseCases;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Presentation.CustomFilters;

public static class CustomServiceExtentions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, UserBlockingHadler>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserService, UserService>();

        return services;
    }
}