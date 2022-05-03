using System.Reflection;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;

namespace FSH.WebApi.Infrastructure.Catalog;

public class ProductSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BrandSeeder> _logger;

    public int SeedOrder { get; set; }

    public ProductSeeder(ISerializerService serializerService, ILogger<BrandSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
        SeedOrder = 1;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!_db.Products.Any())
        {

            

            _logger.LogInformation("Started to Seed Sample Producst.");

            var brands = _db.Brands.ToList();

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string productData = await File.ReadAllTextAsync(path + "/Catalog/products.json", cancellationToken);

            var products = _serializerService.Deserialize<List<Product>>(productData);

            if (brands != null)
            {
                foreach (var product in products)
                {
                    var newProduct = new Product(product.Name, product.Description, product.Rate, GetRandomBrand(brands), product.ImagePath);
                    await _db.Products.AddAsync(newProduct, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Products.");
        }
    }

    private Guid GetRandomBrand(List<Brand> brands)
    {
        Random rnd = new Random();
        var max = brands.Count - 1;
        var index = rnd.Next(0,max);

        return brands[index].Id;
    }
}