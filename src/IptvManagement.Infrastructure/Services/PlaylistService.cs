using System.Text;
using IptvManagement.Application.Abstractions;
using IptvManagement.Application.Common;
using IptvManagement.Domain.Entities;
using IptvManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IptvManagement.Infrastructure.Services;

public class PlaylistService(AppDbContext db, ITokenService tokens, IConfiguration config) : IPlaylistService
{
    public async Task<PlaylistResult> GenerateAsync(string token, string? deviceIdentifier, string? ip, string? userAgent, CancellationToken ct)
    {
        var hash = tokens.HashToken(token);
        var sub = await db.Subscriptions
            .Include(x => x.Customer)
            .Include(x => x.Package).ThenInclude(p => p.PackageChannels).ThenInclude(pc => pc.Channel).ThenInclude(c => c.Category)
            .Include(x => x.Devices)
            .FirstOrDefaultAsync(x => x.PlaylistTokenHash == hash, ct);
        var log = new PlaylistAccessLog { IpAddress = ip, UserAgent = userAgent };
        if (sub is null) return await Fail(log, "Ungültiger Zugriff", 401, ct);
        log.CustomerId = sub.CustomerId;
        log.SubscriptionId = sub.Id;
        var now = DateTime.UtcNow;
        if (!sub.Customer.IsActive || sub.Customer.IsBlocked) return await Fail(log, "Kunde gesperrt", 403, ct);
        if (!sub.IsUsable(now)) return await Fail(log, "Abonnement abgelaufen", 403, ct);
        if (!sub.Package.IsActive) return await Fail(log, "Paket inaktiv", 403, ct);
        if (sub.Devices.Count(d => d.IsActive && !d.IsBlocked) > sub.MaxDevices) return await Fail(log, "Gerätelimit erreicht", 403, ct);
        var baseUrl = config["PublicBaseUrl"]?.TrimEnd('/') ?? "https://domain.example";
        var sb = new StringBuilder("#EXTM3U\n");
        foreach (var pc in sub.Package.PackageChannels.OrderBy(x => x.Channel.Category.SortOrder).ThenBy(x => x.Channel.SortOrder))
        {
            var c = pc.Channel;
            if (!c.IsActive || !c.Category.IsActive || !c.HasValidLicense(now)) continue;
            sb.Append("#EXTINF:-1 tvg-id=\"").Append(c.Id).Append("\" tvg-name=\"").Append(Escape(c.Name)).Append("\" tvg-logo=\"").Append(Escape(c.LogoUrl ?? "")).Append("\" group-title=\"").Append(Escape(c.Category.Name)).Append("\",").Append(Escape(c.Name)).Append('\n');
            sb.Append(baseUrl).Append("/api/streams/").Append(c.Id).Append("?accessToken=").Append(Uri.EscapeDataString(token)).Append('\n');
        }
        log.WasSuccessful = true;
        db.PlaylistAccessLogs.Add(log);
        await db.SaveChangesAsync(ct);
        return new PlaylistResult(true, sb.ToString(), null);
    }

    public async Task<StreamAccessResult> ValidateStreamAccessAsync(Guid channelId, string accessToken, string? ip, CancellationToken ct)
    {
        var hash = tokens.HashToken(accessToken);
        var sub = await db.Subscriptions.Include(x => x.Customer).Include(x => x.Package).ThenInclude(p => p.PackageChannels).ThenInclude(pc => pc.Channel).Include(x => x.Devices).FirstOrDefaultAsync(x => x.PlaylistTokenHash == hash, ct);
        var now = DateTime.UtcNow;
        if (sub is null || !sub.Customer.IsActive || sub.Customer.IsBlocked || !sub.IsUsable(now) || !sub.Package.IsActive) return new(false, null, "Zugriff verweigert", 403);
        var channel = sub.Package.PackageChannels.Select(x => x.Channel).FirstOrDefault(x => x.Id == channelId);
        if (channel is null || !channel.IsActive || !channel.HasValidLicense(now)) return new(false, null, "Zugriff verweigert", 403);
        return new(true, $"signed-origin-url-for-{channelId}", null);
    }

    private async Task<PlaylistResult> Fail(PlaylistAccessLog log, string reason, int code, CancellationToken ct)
    {
        log.WasSuccessful = false;
        log.FailureReason = reason;
        db.PlaylistAccessLogs.Add(log);
        await db.SaveChangesAsync(ct);
        return new PlaylistResult(false, null, "Zugriff verweigert", code);
    }

    private static string Escape(string v) => v.Replace("\"", "'").Replace("\r", "").Replace("\n", "");
}
