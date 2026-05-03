using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Coordinates = MakeSport.Domain.Venues.ValueObjects.Coordinates;

namespace MakeSport.Infrastructure.Postgres.Converters;

public class GeoCoordinateConverter() : ValueConverter<Coordinates, Point>(
    geo => new Point(geo.Longitude, geo.Latitude),
    point => Coordinates.Create(point.Y, point.X));