namespace IptvManagement.Application.Common;
public record SearchRequest(string? Search,int Page=1,int PageSize=20,string? SortBy=null,bool Descending=false);
public record PagedResult<T>(IReadOnlyList<T> Items,int Page,int PageSize,int TotalCount);
public record PlaylistResult(bool Success,string? Content,string? FailureReason,int StatusCode=200);
public record StreamAccessResult(bool Success,string? SignedUrl,string? FailureReason,int StatusCode=200);
public record TokenIssueResult(string PlainToken,DateTime CreatedAtUtc);
