using EliteKit.Application.Registration.Endpoints;

namespace EliteKit.Application.Registration.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        HealthEndpoint sut = new();
        await sut.HandleAsync(new MyRequest()
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 3,
        }, default);
    }
}