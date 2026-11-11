using VenuesService.Domain.Reviews;

namespace VenuesService.Domain.Shared;

public static class ReviewErrors
{
    public static Error RatingOutOfRange() =>
        Error.Validation("review.rating.invalid", "Оценка должна быть от 1 до 5");

    public static Error AlreadyExists() =>
        Error.Conflict("review.already.exists", "Вы уже оставили отзыв об этой площадке");

    public static Error NotFound() =>
        Error.NotFound("review.not.found", "Отзыв не найден");

    public static Error NotAuthor() =>
        Error.Authorization("review.not.author", "Можно изменять только свой отзыв");

    public static Error TextTooLong() =>
        Error.Validation(
            "review.text.too.long",
            $"Отзыв не должен превышать {ReviewsConstraints.TextMaxLength} символов");

    public static Error VenueNotFound(Guid venueId) =>
        GeneralErrors.NotFound(venueId, "площадка");

    public static Error DatabaseError() =>
        Error.Failure("review.database.error", "Ошибка базы данных при работе с отзывом");

    public static Error OperationCancelled() =>
        Error.Failure("review.operation.cancelled", "Операция была отменена");
}