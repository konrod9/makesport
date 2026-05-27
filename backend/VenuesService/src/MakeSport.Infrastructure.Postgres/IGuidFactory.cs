namespace MakeSport.Infrastructure.Postgres;

public interface IGuidFactory
{
    Guid Create();
}

public class GuidFactory : IGuidFactory
{
    public Guid Create()
    {
        return Guid.NewGuid();
    }
}