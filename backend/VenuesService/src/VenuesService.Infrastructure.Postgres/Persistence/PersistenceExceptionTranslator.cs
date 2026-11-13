using Npgsql;
using VenuesService.Domain.Shared;

namespace VenuesService.Infrastructure.Postgres.Persistence;

public static class PersistenceExceptionTranslator
{
    public static Error Translate(PostgresException pg) => pg.SqlState switch
    {
        PostgresErrorCodes.UniqueViolation
            when pg.ConstraintName == DatabaseConstraints.ReviewsUnique =>
            ReviewErrors.AlreadyExists(),

        PostgresErrorCodes.ForeignKeyViolation
            when pg.ConstraintName == DatabaseConstraints.ReviewsVenueFk =>
            GeneralErrors.NotFound(null, "площадка"),

        PostgresErrorCodes.CheckViolation
            when pg.ConstraintName == DatabaseConstraints.ReviewsRatingCheck =>
            GeneralErrors.ValueIsInvalid("rating"),

        PostgresErrorCodes.CheckViolation
            when pg.ConstraintName == DatabaseConstraints.ReviewsTextLengthCheck =>
            ReviewErrors.TextTooLong(),

        _ => GeneralErrors.DatabaseError(),
    };
}