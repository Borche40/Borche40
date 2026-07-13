using IptvManagement.Domain.Entities; using IptvManagement.Domain.Enums; using IptvManagement.Infrastructure.Security; using Microsoft.AspNetCore.DataProtection;
namespace IptvManagement.UnitTests;
public class DomainWorkflowTests{
 [Fact] public void ActiveSubscription_IsUsable_UntilExpiration(){ var s=new Subscription{Status=SubscriptionStatus.Active,ExpirationDateUtc=DateTime.UtcNow.AddDays(1)}; Assert.True(s.IsUsable(DateTime.UtcNow)); }
 [Fact] public void ExpiredSubscription_IsNotUsable(){ var s=new Subscription{Status=SubscriptionStatus.Active,ExpirationDateUtc=DateTime.UtcNow.AddMinutes(-1)}; Assert.False(s.IsUsable(DateTime.UtcNow)); }
 [Fact] public void Renewal_UsesExistingExpiration_WhenStillActive(){ var now=new DateTime(2026,7,13,0,0,0,DateTimeKind.Utc); var s=new Subscription{ExpirationDateUtc=now.AddMonths(1),Status=SubscriptionStatus.Active}; s.Renew(12,now); Assert.Equal(now.AddMonths(13),s.ExpirationDateUtc); Assert.Equal(SubscriptionStatus.Active,s.Status); }
 [Fact] public void Renewal_UsesCurrentUtc_WhenExpired(){ var now=new DateTime(2026,7,13,0,0,0,DateTimeKind.Utc); var s=new Subscription{ExpirationDateUtc=now.AddDays(-1),Status=SubscriptionStatus.Expired}; s.Renew(1,now); Assert.Equal(now.AddMonths(1),s.ExpirationDateUtc); }
 [Fact] public void TokenHash_Verifies_AndDoesNotStorePlainToken(){ var service=new TokenService(); var token=service.CreateSecureToken(); var hash=service.HashToken(token); Assert.NotEqual(token,hash); Assert.True(service.Verify(token,hash)); Assert.False(service.Verify(token+"x",hash)); }
 [Fact] public void InvoiceTaxCalculation_IsDeterministic(){ var net=100m; var tax=Math.Round(net*20m/100m,2); Assert.Equal(20m,tax); Assert.Equal(120m,net+tax); }
 [Fact] public void ChannelLicense_AllowsPublicOrValidLicensed(){ var now=DateTime.UtcNow; Assert.True(new Channel{IsPubliclyAvailable=true}.HasValidLicense(now)); Assert.True(new Channel{LicenseValidUntilUtc=now.AddDays(1)}.HasValidLicense(now)); Assert.False(new Channel{LicenseValidUntilUtc=now.AddDays(-1)}.HasValidLicense(now)); }
}
