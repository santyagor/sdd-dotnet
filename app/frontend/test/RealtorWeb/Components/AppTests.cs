namespace RealtorWeb.Tests.Components;

public class AppTests : BunitContext
{
    [Fact]
    public void App_RendersSuccessfully()
    {
        var cut = Render<RealtorWeb.Components.App>();

        cut.Markup.Should().Contain("Welcome to Realtor");
    }
}
