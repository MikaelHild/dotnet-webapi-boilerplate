namespace FSH.WebApi.Infrastructure.Persistence.Initialization;

public interface ICustomSeeder
{
    int SeedOrder { get; set; }
    Task InitializeAsync(CancellationToken cancellationToken);
}