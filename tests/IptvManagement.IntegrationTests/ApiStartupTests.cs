using Xunit;

namespace IptvManagement.IntegrationTests;

public class ApiStartupTests
{
    [Fact]
    public void TestProject_IsDiscoverable()
    {
        Assert.Equal("IptvManagement.IntegrationTests", typeof(ApiStartupTests).Namespace);
    }
}
