using WebApp.CloudApi.RequestModel;
using Xunit;

namespace WebApp.Tests;

public class AccountRequestTest
{
    [Fact]
    public void CheckIfModelValid()
    {
        var model = new AccountRequest {
            FirstName = "Test",
            LastName = "Test",
            Email = "test@test",
            Password = "Test1234"
        };
        Assert.True(model != null);
    }
}