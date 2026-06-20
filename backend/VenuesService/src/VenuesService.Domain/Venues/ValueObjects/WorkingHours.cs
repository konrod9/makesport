using CSharpFunctionalExtensions;
using VenuesService.Domain.Shared;

namespace VenuesService.Domain.Venues.ValueObjects;

public record WorkingHours
{
    private WorkingHours(string workingStart, string workingEnd)
    {
        WorkingStart = workingStart;
        WorkingEnd = workingEnd;
    }
    
    public string WorkingStart { get; }

    public string WorkingEnd { get; }

    public string Value => string.IsNullOrWhiteSpace(WorkingStart) || string.IsNullOrWhiteSpace(WorkingEnd)
        ? "Круглосуточно"
        : $"{WorkingStart} - {WorkingEnd}";

    public static Result<WorkingHours, Error> Create(string workingStart, string workingEnd)
    {
        return new WorkingHours(workingStart, workingEnd);
    }
}