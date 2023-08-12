using Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Presentation.CustomFilters;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        IConfiguration config = builder.Configuration;
        Log.Logger = CreateSerilogLogger(config);

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        builder.Services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("Default"), options =>
            {
                options.EnableRetryOnFailure(3);
            });
        });

        builder.Services.AddCustomServices();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new("/Identity/Login");
                options.LogoutPath = new("/Identity/Logout");
                options.AccessDeniedPath = new("/Identity/Login");
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Default", policy => 
                policy.Requirements.Add(new UserBlockingRequirement()));
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            using (var appContext = scope.ServiceProvider.GetRequiredService<UserDbContext>())
            {
                try
                {
                    appContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Migration proccess failed");
                }
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Identity/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Identity}/{action=Login}/{id?}");

        app.Run();
    }

    private static Serilog.ILogger CreateSerilogLogger(IConfiguration config)
    {
        //var seqServerUrl = config["Serilog:SeqServerUrl"];
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            //.WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
            .ReadFrom.Configuration(config)
            .CreateLogger();
    }
}