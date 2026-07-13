using Microsoft.AspNetCore.Identity;
namespace IptvManagement.Infrastructure.Identity;
public class ApplicationUser:IdentityUser { public string FirstName{get;set;}=""; public string LastName{get;set;}=""; public bool IsActive{get;set;}=true; public DateTime CreatedAtUtc{get;set;}=DateTime.UtcNow; public DateTime? LastLoginAtUtc{get;set;} }
