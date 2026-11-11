using Modsen.Portal.Core.Database;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Seeds;

internal class SizesSeed : ISeedEntitiesProvider<MerchDbContext>
{
    private readonly MerchDbContext _dbContext;

    public SizesSeed(MerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var sizes = new List<MerchSizeEntity>
        {
            new() { Id = "XS", Name = "Extra small", OrderWeight = 100, IsForClothing = true },
            new() { Id = "S", Name = "Small", OrderWeight = 200, IsForClothing = true },
            new() { Id = "M", Name = "Medium", OrderWeight = 300, IsForClothing = true },
            new() { Id = "L", Name = "Large", OrderWeight = 400, IsForClothing = true },
            new() { Id = "XL", Name = "Extra large", OrderWeight = 500, IsForClothing = true },
            new() { Id = "2XL", Name = "Double extra large", OrderWeight = 600, IsForClothing = true },
            new() { Id = "3XL", Name = "Triple extra large", OrderWeight = 700, IsForClothing = true },
            new() { Id = "4XL", Name = "Quadruple extra large", OrderWeight = 800, IsForClothing = true },
            new() { Id = "One", Name = "One size", OrderWeight = 0, IsForClothing = false }
        };

        foreach (var size in sizes)
        {
            var entity = _dbContext.MerchSizes.FirstOrDefault(sizeEntity =>
                sizeEntity.Id == size.Id ||
                sizeEntity.Name == size.Name);

            if (entity is null)
            {
                _dbContext.MerchSizes.Add(size);
            }
        }

        _dbContext.SaveChanges();
    }
}
