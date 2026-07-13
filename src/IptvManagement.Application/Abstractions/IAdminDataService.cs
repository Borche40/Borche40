using IptvManagement.Application.DTOs;
using IptvManagement.Domain.Entities;
using IptvManagement.Domain.Enums;

namespace IptvManagement.Application.Abstractions;

public interface IAdminDataService
{
    Task<IReadOnlyList<Customer>> GetCustomersAsync(string? search, string? filter, CancellationToken ct); Task SaveCustomerAsync(CustomerFormModel model, string? userId, CancellationToken ct); Task DeactivateCustomerAsync(Guid id, CancellationToken ct); Task SetCustomerBlockedAsync(Guid id, bool blocked, CancellationToken ct);
    Task<IReadOnlyList<ChannelCategory>> GetCategoriesAsync(CancellationToken ct); Task SaveCategoryAsync(CategoryFormModel model, CancellationToken ct); Task DeactivateCategoryAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<SubscriptionPackage>> GetPackagesAsync(CancellationToken ct); Task SavePackageAsync(PackageFormModel model, CancellationToken ct); Task DeactivatePackageAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken ct); Task SaveChannelAsync(ChannelFormModel model, CancellationToken ct); Task DeactivateChannelAsync(Guid id, CancellationToken ct); Task<bool> TestStreamAsync(string url, CancellationToken ct);
    Task<IReadOnlyList<Subscription>> GetSubscriptionsAsync(CancellationToken ct); Task SaveSubscriptionAsync(SubscriptionFormModel model, CancellationToken ct); Task SetSubscriptionStatusAsync(Guid id, SubscriptionStatus status, CancellationToken ct); Task RenewSubscriptionAsync(Guid id, int months, CancellationToken ct); Task<string> IssueTokenAsync(Guid id, CancellationToken ct); Task RevokeTokenAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Device>> GetDevicesAsync(CancellationToken ct); Task SaveDeviceAsync(DeviceFormModel model, CancellationToken ct); Task SetDeviceBlockedAsync(Guid id, bool blocked, CancellationToken ct); Task DeactivateDeviceAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken ct); Task SavePaymentAsync(PaymentFormModel model, CancellationToken ct);
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct); Task<InvoiceDto> CreateInvoiceAsync(InvoiceFormModel model, CancellationToken ct); Task MarkInvoicePaidAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<AuditLog>> GetAuditLogsAsync(CancellationToken ct); Task<IReadOnlyList<SystemSetting>> GetSettingsAsync(CancellationToken ct); Task SaveSettingAsync(Guid? id, string key, string value, string? description, CancellationToken ct);
    Task<IReadOnlyList<UserListModel>> GetUsersAsync(CancellationToken ct); Task SaveUserAsync(UserFormModel model, CancellationToken ct); Task SetUserActiveAsync(string id, bool active, CancellationToken ct);
}
