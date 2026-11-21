using VenuesService.Domain.Shared;

namespace VenuesService.IntegrationTests.Infrastructure;

public sealed record TestEnvelope<T>(T? Result, Error? Error, bool IsError);

public sealed record TestEnvelope(Error? Error, bool IsError);