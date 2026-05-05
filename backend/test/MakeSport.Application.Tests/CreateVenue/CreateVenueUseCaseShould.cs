namespace MakeSport.Application.Tests.CreateVenue;

public class CreateVenueUseCaseShould
{
    /*private readonly CreateVenueUseCase sut;
    private readonly Mock<ICreateVenueStorage> storage;
    private readonly ISetup<ICreateVenueStorage, Task<VenueDto>> createVenueSetup;
    private readonly Mock<IValidator<CreateVenueCommand>> validator;
    private readonly IReturnsResult<IValidator<CreateVenueCommand>> validatorSetup;*/

    /*public CreateVenueUseCaseShould()
    {
        storage = new Mock<ICreateVenueStorage>();
        createVenueSetup = storage.Setup(s => 
            s.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()));
        
        validator = new Mock<IValidator<CreateVenueCommand>>();
        validatorSetup = validator.Setup(v => 
            v.ValidateAsync(It.IsAny<CreateVenueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        //sut = new CreateVenueUseCase(storage.Object, validator.Object);
    }

    [Fact]
    public async Task ReturnNewlyCreatedVenue_WhenVenueIsCreated()
    {
        var venueId = Guid.Parse("A2426F6B-BB0F-495A-8934-30027FADF3BB");

        var expectedVenue = new VenueDto(venueId, "Name", "Description", 60.1234, 60.1234);
        createVenueSetup.ReturnsAsync(expectedVenue);
        
        var actual = await sut.Handle(new CreateVenueCommand("Name", "Description", 60.1234, 60.1234), CancellationToken.None);
        actual.Should().Be(expectedVenue);
        
        storage.Verify(s => 
            s.CreateAsync("Name", "Description", 60.1234, 60.1234, It.IsAny<CancellationToken>()), Times.Once);
    }*/
}