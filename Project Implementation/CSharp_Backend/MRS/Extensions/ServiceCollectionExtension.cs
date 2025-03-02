
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRS.Core.Contracts;
using MRS.Core.Service;
using MRS.Infrastructure.Data;
using MRS.Infrastructure.Data.Repositories;


namespace MRS.Extensions;

using Microsoft.AspNetCore.Identity.UI.Services;
using MRS.Infrastructure.Data.Identity;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IApplicatioDbRepository, ApplicatioDbRepository>();
        services.AddScoped<ILoadService, LoadService>();
        services.AddTransient<IMovieService, MovieService>();
        

        return services;
    }

    public static IServiceCollection AddApplicationDbContexts(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = "Data Source=.;Initial Catalog=MRS;Integrated Security=SSPI;TrustServerCertificate=True;User Id=Web;Password=123";
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}
