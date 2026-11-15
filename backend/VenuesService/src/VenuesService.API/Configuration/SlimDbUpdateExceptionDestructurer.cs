using Microsoft.EntityFrameworkCore;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;

namespace VenuesService.API.Configuration;

public sealed class SlimDbUpdateExceptionDestructurer : ExceptionDestructurer
{
    public override Type[] TargetTypes => [typeof(DbUpdateException)];

    public override void Destructure(
        Exception exception,
        IExceptionPropertiesBag propertiesBag,
        Func<Exception, IReadOnlyDictionary<string, object?>?> destructureException)
    {
        base.Destructure(exception, propertiesBag, destructureException);

        var entries = ((DbUpdateException)exception).Entries
            .Select(e => $"{e.Metadata.ClrType.Name} ({e.State}): " +
                         string.Join(", ", e.Properties
                             .Where(p => p.IsModified)
                             .Select(p => p.Metadata.Name)))
            .ToArray();

        propertiesBag.AddProperty("Entries", entries);
    }
}