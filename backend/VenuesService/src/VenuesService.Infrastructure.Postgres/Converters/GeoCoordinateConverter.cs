using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Coordinates = VenuesService.Domain.Venues.ValueObjects.Coordinates;

namespace VenuesService.Infrastructure.Postgres.Converters;

/*public class GeoCoordinateConverter() : ValueConverter<Coordinates, Point>(
    geo => new Point(geo.Longitude, geo.Latitude),
    point => Coordinates.Create(point.Y, point.X));*/