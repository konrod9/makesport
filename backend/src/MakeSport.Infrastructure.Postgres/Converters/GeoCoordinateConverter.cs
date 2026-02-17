using MakeSport.Domain.Venues;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace MakeSport.Infrastructure.Postgres.Converters;

public class GeoCoordinateConverter() : ValueConverter<GeoCoordinate, Point>(
    geo => new Point(geo.Longitude, geo.Latitude),
    point => new GeoCoordinate(point.Y, point.X));