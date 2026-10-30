namespace Wadatika.Api.Contracts;

public sealed record HealthResponse(
     string Status,
     string Message,
     DateTimeOffset Timestamp
);