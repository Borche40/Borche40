using IptvManagement.Application.Common; using IptvManagement.Application.DTOs;
namespace IptvManagement.Application.Abstractions;
public interface ITokenService { string CreateSecureToken(); string HashToken(string token); bool Verify(string token,string hash); }
public interface IStreamUrlProtector { string Protect(string streamUrl); string Unprotect(string encrypted); string Mask(string streamUrl); }
public interface IPlaylistService { Task<PlaylistResult> GenerateAsync(string token,string? deviceIdentifier,string? ip,string? userAgent,CancellationToken cancellationToken); Task<StreamAccessResult> ValidateStreamAccessAsync(Guid channelId,string accessToken,string? ip,CancellationToken cancellationToken); }
public interface ISubscriptionService { Task<TokenIssueResult> IssuePlaylistTokenAsync(Guid subscriptionId,CancellationToken cancellationToken); Task RenewAsync(Guid subscriptionId,int months,string? userId,CancellationToken cancellationToken); Task RevokePlaylistTokenAsync(Guid subscriptionId,CancellationToken cancellationToken); }
public interface IInvoiceService { Task<InvoiceDto> CreateAsync(Guid customerId,Guid subscriptionId,decimal netAmount,CancellationToken cancellationToken); byte[] RenderPdf(InvoiceDto invoice); }
public interface IDashboardService { Task<DashboardDto> GetAsync(CancellationToken cancellationToken); }
public interface ICrudService<TDto,TCreate,TUpdate> { Task<PagedResult<TDto>> SearchAsync(SearchRequest request,CancellationToken ct); Task<TDto> GetAsync(Guid id,CancellationToken ct); Task<TDto> CreateAsync(TCreate request,CancellationToken ct); Task<TDto> UpdateAsync(Guid id,TUpdate request,CancellationToken ct); Task DeactivateAsync(Guid id,CancellationToken ct); }
