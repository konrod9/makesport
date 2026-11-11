using System.Text.Json.Serialization;

namespace VenuesService.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewStatus
{
    Published,
    Hidden
}