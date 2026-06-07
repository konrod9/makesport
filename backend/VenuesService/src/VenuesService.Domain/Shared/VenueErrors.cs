namespace VenuesService.Domain.Shared;

public static class VenueErrors
{
    public static Error TitleConflict(string title) =>
        Error.Conflict("venue.title.conflict", $"Площадка с названием {title} уже существует");
    
    public static Error DatabaseError() =>
        Error.Failure("venue.database.error", "Ошибка базы данных при работе с сервисом 'Venues'");
    
    public static Error OperationCancelled() =>
        Error.Failure("venue.operation.cancelled", "Операция была отменена");
}