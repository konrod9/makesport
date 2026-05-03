using CSharpFunctionalExtensions;
using MakeSport.Contracts.Dtos;
using MakeSport.Contracts.Requests;

namespace MakeSport.Application.UseCases.CreateVenue;

public interface ICreateVenueUseCase
{
    Task<Result<VenueDto, string>> Handle(CreateVenueRequest request, CancellationToken cancellationToken = default);
}