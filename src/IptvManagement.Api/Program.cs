using System.Threading.RateLimiting;
using IptvManagement.Infrastructure;
using IptvManagement.Infrastructure.Data;
using IptvManagement.Infrastructure.Startup;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("playlist", limiterOptions =>
{
    limiterOptions.PermitLimit = 60;
    limiterOptions.Window = TimeSpan.FromMinutes(1);
    limiterOptions.QueueLimit = 0;
    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageContent", policy => policy.RequireRole("Administrator", "Mitarbeiter"));
    options.AddPolicy("ReadOnly", policy => policy.RequireRole("Administrator", "Mitarbeiter", "Reseller", "NurLesen"));
});

var app = builder.Build();
await DatabaseInitializer.InitializeAsync(app.Services, app.Environment);

app.UseExceptionHandler();
app.UseHsts();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; frame-ancestors 'none'";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.ReferrerPolicy = "no-referrer";
    await next();
});
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();

public partial class Program { }
