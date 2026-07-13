using System.Globalization;
using System.Text;
using IptvManagement.Application.Abstractions;
using IptvManagement.Application.Common;
using IptvManagement.Application.DTOs;
using IptvManagement.Domain.Entities;
using IptvManagement.Domain.Enums;
using IptvManagement.Domain.Exceptions;
using IptvManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IptvManagement.Infrastructure.Services;

/// <summary>
/// Verwaltet sicherheitsrelevante Abonnementaktionen wie Token-Ausstellung, Widerruf und Verlängerung.
/// </summary>
public sealed class SubscriptionService(AppDbContext db, ITokenService tokens) : ISubscriptionService
{
    public async Task<TokenIssueResult> IssuePlaylistTokenAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await db.Subscriptions.FindAsync([subscriptionId], cancellationToken)
            ?? throw new NotFoundException("Abonnement wurde nicht gefunden.");

        var token = tokens.CreateSecureToken();
        subscription.PlaylistTokenHash = tokens.HashToken(token);
        subscription.TokenCreatedAtUtc = DateTime.UtcNow;
        subscription.UpdatedAtUtc = subscription.TokenCreatedAtUtc;

        db.AuditLogs.Add(new AuditLog
        {
            Action = "Playlist-Token erstellt",
            EntityName = nameof(Subscription),
            EntityId = subscriptionId.ToString()
        });

        await db.SaveChangesAsync(cancellationToken);
        return new TokenIssueResult(token, subscription.TokenCreatedAtUtc.Value);
    }

    public async Task RenewAsync(Guid subscriptionId, int months, string? userId, CancellationToken cancellationToken)
    {
        if (months <= 0)
        {
            throw new ValidationException("Die Verlängerung muss mindestens einen Monat betragen.");
        }

        var subscription = await db.Subscriptions.FindAsync([subscriptionId], cancellationToken)
            ?? throw new NotFoundException("Abonnement wurde nicht gefunden.");

        subscription.Renew(months, DateTime.UtcNow);
        db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = $"Abonnement um {months.ToString(CultureInfo.InvariantCulture)} Monate verlängert",
            EntityName = nameof(Subscription),
            EntityId = subscriptionId.ToString()
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokePlaylistTokenAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await db.Subscriptions.FindAsync([subscriptionId], cancellationToken)
            ?? throw new NotFoundException("Abonnement wurde nicht gefunden.");

        subscription.PlaylistTokenHash = null;
        subscription.UpdatedAtUtc = DateTime.UtcNow;

        db.AuditLogs.Add(new AuditLog
        {
            Action = "Playlist-Token widerrufen",
            EntityName = nameof(Subscription),
            EntityId = subscriptionId.ToString()
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Erstellt Rechnungen und erzeugt eine einfache, stabile PDF-Nutzlast für Download-Endpunkte.
/// </summary>
public sealed class InvoiceService(AppDbContext db) : IInvoiceService
{
    public async Task<InvoiceDto> CreateAsync(Guid customerId, Guid subscriptionId, decimal netAmount, CancellationToken cancellationToken)
    {
        if (netAmount < 0)
        {
            throw new ValidationException("Der Nettobetrag darf nicht negativ sein.");
        }

        var taxRateSetting = await db.SystemSettings.FirstOrDefaultAsync(x => x.Key == "Tax:VatRate", cancellationToken);
        var taxRate = decimal.TryParse(taxRateSetting?.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var configuredRate)
            ? configuredRate
            : 20m;
        var taxAmount = Math.Round(netAmount * taxRate / 100m, 2, MidpointRounding.AwayFromZero);
        var invoice = new Invoice
        {
            CustomerId = customerId,
            SubscriptionId = subscriptionId,
            InvoiceNumber = $"RE-{DateTime.UtcNow:yyyyMMdd}-{await db.Invoices.CountAsync(cancellationToken) + 1:0000}",
            InvoiceDateUtc = DateTime.UtcNow,
            DueDateUtc = DateTime.UtcNow.AddDays(14),
            NetAmount = netAmount,
            TaxAmount = taxAmount,
            GrossAmount = netAmount + taxAmount,
            Currency = "EUR",
            Status = InvoiceStatus.Issued
        };

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync(cancellationToken);

        return new InvoiceDto(
            invoice.Id,
            invoice.CustomerId,
            invoice.SubscriptionId,
            invoice.InvoiceNumber,
            invoice.InvoiceDateUtc,
            invoice.DueDateUtc,
            invoice.NetAmount,
            invoice.TaxAmount,
            invoice.GrossAmount,
            invoice.Currency,
            invoice.Status,
            invoice.PdfFilePath);
    }

    public byte[] RenderPdf(InvoiceDto invoice)
    {
        var content = $"Rechnung {invoice.InvoiceNumber}\nNetto {invoice.NetAmount:n2} {invoice.Currency}\nUSt {invoice.TaxAmount:n2}\nBrutto {invoice.GrossAmount:n2} {invoice.Currency}";
        return Encoding.UTF8.GetBytes(content);
    }
}

/// <summary>
/// Liefert Dashboard-Kennzahlen ausschließlich aus gespeicherten Daten.
/// </summary>
public sealed class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var expiringIn7Days = await CreateSubscriptionQuery(now, now.AddDays(7)).ToListAsync(cancellationToken);
        var expiringIn30Days = await CreateSubscriptionQuery(now, now.AddDays(30)).ToListAsync(cancellationToken);
        var lastPayments = await db.Payments
            .OrderByDescending(payment => payment.PaymentDateUtc)
            .Take(10)
            .Select(payment => new PaymentDto(payment.Id, payment.CustomerId, payment.SubscriptionId, payment.PaymentNumber, payment.Amount, payment.Currency, payment.PaymentMethod, payment.PaymentDateUtc, payment.Status, payment.Reference))
            .ToListAsync(cancellationToken);
        var lastActivities = await db.AuditLogs
            .OrderByDescending(audit => audit.CreatedAtUtc)
            .Take(20)
            .Select(audit => new AuditLogDto(audit.Id, audit.UserId, audit.Action, audit.EntityName, audit.EntityId, audit.IpAddress, audit.CreatedAtUtc))
            .ToListAsync(cancellationToken);
        var revenueByMonth = await LoadRevenueByMonthAsync(now, cancellationToken);

        return new DashboardDto(
            await db.Customers.CountAsync(cancellationToken),
            await db.Customers.CountAsync(customer => customer.IsActive, cancellationToken),
            await db.Customers.CountAsync(customer => customer.IsBlocked, cancellationToken),
            await db.Subscriptions.CountAsync(subscription => subscription.Status == SubscriptionStatus.Active && subscription.ExpirationDateUtc > now, cancellationToken),
            await db.Subscriptions.CountAsync(subscription => subscription.ExpirationDateUtc <= now.AddDays(30) && subscription.ExpirationDateUtc > now, cancellationToken),
            await db.Subscriptions.CountAsync(subscription => subscription.ExpirationDateUtc <= now, cancellationToken),
            await db.Payments.Where(payment => payment.Status == PaymentStatus.Paid && payment.PaymentDateUtc >= monthStart).SumAsync(payment => (decimal?)payment.Amount, cancellationToken) ?? 0m,
            await db.Payments.Where(payment => payment.Status == PaymentStatus.Open).SumAsync(payment => (decimal?)payment.Amount, cancellationToken) ?? 0m,
            await db.Devices.CountAsync(device => device.IsActive, cancellationToken),
            await db.Devices.CountAsync(device => device.IsBlocked, cancellationToken),
            await db.Channels.CountAsync(channel => channel.IsActive, cancellationToken),
            expiringIn7Days,
            expiringIn30Days,
            lastPayments,
            lastActivities,
            revenueByMonth);
    }

    private IQueryable<SubscriptionDto> CreateSubscriptionQuery(DateTime fromUtc, DateTime untilUtc)
    {
        return db.Subscriptions
            .Where(subscription => subscription.ExpirationDateUtc <= untilUtc && subscription.ExpirationDateUtc > fromUtc)
            .OrderBy(subscription => subscription.ExpirationDateUtc)
            .Take(20)
            .Select(subscription => new SubscriptionDto(subscription.Id, subscription.CustomerId, subscription.PackageId, subscription.SubscriptionNumber, subscription.StartDateUtc, subscription.ExpirationDateUtc, subscription.Status, subscription.MaxDevices, subscription.AutomaticRenewal));
    }

    private async Task<IReadOnlyList<decimal>> LoadRevenueByMonthAsync(DateTime now, CancellationToken cancellationToken)
    {
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);
        var monthly = await db.Payments
            .Where(payment => payment.Status == PaymentStatus.Paid && payment.PaymentDateUtc >= start)
            .GroupBy(payment => new { payment.PaymentDateUtc.Year, payment.PaymentDateUtc.Month })
            .Select(group => new { group.Key.Year, group.Key.Month, Amount = group.Sum(payment => (decimal?)payment.Amount) ?? 0m })
            .ToListAsync(cancellationToken);

        return Enumerable.Range(0, 12)
            .Select(offset => start.AddMonths(offset))
            .Select(month => monthly.FirstOrDefault(item => item.Year == month.Year && item.Month == month.Month)?.Amount ?? 0m)
            .ToArray();
    }
}
