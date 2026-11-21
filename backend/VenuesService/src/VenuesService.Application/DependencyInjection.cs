using FileService.Contracts.HttpCommunication;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VenuesService.Application.UseCases.CreateReview;
using VenuesService.Application.UseCases.CreateVenue;
using VenuesService.Application.UseCases.DeleteReview;
using VenuesService.Application.UseCases.GetById;
using VenuesService.Application.UseCases.GetCities;
using VenuesService.Application.UseCases.GetMyVenues;
using VenuesService.Application.UseCases.GetVenueReviews;
using VenuesService.Application.UseCases.GetVenues;
using VenuesService.Application.UseCases.UpdateReview;

namespace VenuesService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddVenuesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<GetVenuesUseCase>()
            .AddScoped<CreateVenueUseCase>()
            .AddScoped<GetCitiesUseCase>()
            .AddScoped<GetMyVenuesUseCase>()
            .AddScoped<GetByIdUseCase>()
            .AddScoped<CreateReviewUseCase>()
            .AddScoped<GetVenueReviewsUseCase>()
            .AddScoped<UpdateVenueReviewUseCase>()
            .AddScoped<DeleteVenueReviewUseCase>();
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddFileServiceHttpCommunication(configuration);

        return services;
    }
}