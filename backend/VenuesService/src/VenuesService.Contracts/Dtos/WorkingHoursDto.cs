namespace VenuesService.Contracts.Dtos;

public record WorkingHoursDto
{
    public string WorkingStart { get; init; } = string.Empty;

    public string WorkingEnd { get; init; } = string.Empty;
};