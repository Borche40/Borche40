using IptvManagement.Infrastructure;
using IptvManagement.Infrastructure.Startup;
using IptvManagement.Web.Components;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();
await DatabaseInitializer.InitializeAsync(app.Services, app.Environment);

app.UseExceptionHandler("/Fehler");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
