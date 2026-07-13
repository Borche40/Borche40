using IptvManagement.Application.Abstractions; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace IptvManagement.Api.Controllers;
[ApiController][Route("api/dashboard")][Authorize(Policy="ReadOnly")]
public class DashboardController(IDashboardService dashboard):ControllerBase{ [HttpGet] public Task<IptvManagement.Application.DTOs.DashboardDto> Get(CancellationToken ct)=>dashboard.GetAsync(ct); }
