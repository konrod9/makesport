using FluentAssertions;
using MakeSport.Application.UseCases.CreateVenue;

namespace MakeSport.Application.Tests.CreateVenue;

public class CreateVenueCommandValidatorShould
{
    private readonly CreateVenueRequestValidator sut = new();

    /*[Fact]
    public void ReturnSuccess_WhenCommandIsValid()
    {
        var validCommand = new CreateVenueCommand("Valid-name", "Valid-description", 60.0123, 59.0123);
        sut.Validate(validCommand);
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateVenueCommand("Valid-name", "Valid-description", 60.0123, 59.0123);
        yield return [validCommand with { Title = string.Empty }];
        // Name with more than 50 length
        yield return [validCommand with { Title = "012345678901234567890123456789012345678901234567890"}];
        // Description with more than 100 length
        yield return [validCommand with { Description = "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"}];
    }*/

    /*[Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandIsInvalid(CreateVenueCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }*/

}