using FluentValidation;
using RealtorApi.Features.Properties.ListProperties;

namespace RealtorApiTests.UnitTests.Features.Properties.ListProperties;

public class ListPropertiesValidatorTests
{
    private readonly ListPropertiesValidator _validator = new();

    [Fact]
    public void Validate_WhenValuesArePositive_Passes()
    {
        var result = _validator.Validate(new ListPropertiesQuery(1, 6));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 6, true, false)]
    [InlineData(-1, 6, true, false)]
    [InlineData(1, 0, false, true)]
    [InlineData(1, -10, false, true)]
    public void Validate_WhenValuesAreInvalid_Fails(int page, int pageSize, bool expectPageError, bool expectPageSizeError)
    {
        var result = _validator.Validate(new ListPropertiesQuery(page, pageSize));

        if (expectPageError)
        {
            result.Errors.Should().Contain(error => error.PropertyName == nameof(ListPropertiesQuery.Page));
        }
        else
        {
            result.Errors.Should().NotContain(error => error.PropertyName == nameof(ListPropertiesQuery.Page));
        }

        if (expectPageSizeError)
        {
            result.Errors.Should().Contain(error => error.PropertyName == nameof(ListPropertiesQuery.PageSize));
        }
        else
        {
            result.Errors.Should().NotContain(error => error.PropertyName == nameof(ListPropertiesQuery.PageSize));
        }
    }
}
