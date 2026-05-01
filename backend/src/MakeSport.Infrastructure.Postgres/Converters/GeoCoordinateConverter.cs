using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace MakeSport.Infrastructure.Postgres.Converters;

public class GeoCoordinateConverter() : ValueConverter<GeoCoordinates, Point>(
    geo => new Point(geo.Longitude, geo.Latitude),
    point => GeoCoordinates.Create(point.Y, point.X));