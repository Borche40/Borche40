using IptvManagement.Domain.Entities;
using IptvManagement.Infrastructure.Data;
using IptvManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IptvManagement.Infrastructure.Startup;

/// <summary>
/// Führt die Datenbankinitialisierung in der korrekten Startreihenfolge aus.
/// </summary>
public static class DatabaseInitializer
{
    private static readonly string[] RoleNames = ["Administrator", "Mitarbeiter", "Reseller", "NurLesen"];

    public static async Task InitializeAsync(IServiceProvider serviceProvider, IHostEnvironment environment, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var scopedProvider = scope.ServiceProvider;
        var dbContext = scopedProvider.GetRequiredService<AppDbContext>();
        var logger = scopedProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");

        if (environment.IsDevelopment())
        {
            logger.LogInformation("Wende EF-Core-Migrationen für die Entwicklungsdatenbank an.");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        await SeedSystemDataAsync(dbContext, cancellationToken);
        await SeedRolesAsync(scopedProvider.GetRequiredService<RoleManager<IdentityRole>>());
        await SeedInitialAdministratorAsync(scopedProvider.GetRequiredService<UserManager<ApplicationUser>>(), scopedProvider.GetRequiredService<IConfiguration>(), logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedInitialAdministratorAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
    {
        var email = configuration["InitialAdmin:Email"];
        var password = configuration["InitialAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Kein InitialAdmin angelegt, weil InitialAdmin:Email oder InitialAdmin:Password nicht konfiguriert ist.");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, "Administrator"))
            {
                await userManager.AddToRoleAsync(existingUser, "Administrator");
            }

            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Development",
            LastName = "Administrator",
            IsActive = true
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Der InitialAdmin konnte nicht erstellt werden: {errors}");
        }

        await userManager.AddToRoleAsync(user, "Administrator");
        logger.LogInformation("InitialAdmin {Email} wurde angelegt.", email);
    }

    private static async Task SeedSystemDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!await dbContext.SystemSettings.AnyAsync(cancellationToken))
        {
            dbContext.SystemSettings.AddRange(
                new SystemSetting { Key = "Tax:VatRate", Value = "20", Description = "Österreichischer Standard-Umsatzsteuersatz in Prozent." },
                new SystemSetting { Key = "Company:Country", Value = "AT", Description = "Standardland des Unternehmens." });
        }

        if (!await dbContext.Categories.AnyAsync(cancellationToken))
        {
            dbContext.Categories.AddRange(
                new ChannelCategory { Name = "Öffentlich", Description = "Öffentlich verfügbare oder eigene Inhalte", SortOrder = 10 },
                new ChannelCategory { Name = "Eigene Medien", Description = "Selbst produzierte oder ordnungsgemäß lizenzierte Inhalte", SortOrder = 20 });
        }

        if (!await dbContext.Packages.AnyAsync(cancellationToken))
        {
            dbContext.Packages.AddRange(
                new SubscriptionPackage { Name = "Basis", Description = "Basispaket für lizenzierte Inhalte", DurationInMonths = 1, MaxDevices = 1, Price = 9.90m },
                new SubscriptionPackage { Name = "Standard", Description = "Standardpaket für lizenzierte Inhalte", DurationInMonths = 1, MaxDevices = 3, Price = 19.90m },
                new SubscriptionPackage { Name = "Premium", Description = "Premiumpaket für lizenzierte Inhalte", DurationInMonths = 1, MaxDevices = 5, Price = 29.90m });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
