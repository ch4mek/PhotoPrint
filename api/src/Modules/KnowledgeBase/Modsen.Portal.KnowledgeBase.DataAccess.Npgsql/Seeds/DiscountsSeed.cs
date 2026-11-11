using System.Text.Json;
using Bogus;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Seeds;

internal class DiscountsSeed : ISeedEntitiesProvider<KnowledgeBaseDbContext>
{
    private readonly KnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    private readonly string[] _categories = ["Shopping", "Entertainment", "Sport", "Cafe"];

    public DiscountsSeed(KnowledgeBaseDbContext knowledgeBaseDbContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsDevelopmentOrQaEnvironment() &&
            !_knowledgeBaseDbContext.Discounts.Any())
        {
            var faker = new Faker();
            var discountCities = _knowledgeBaseDbContext.Cities.ToArray();
            var tags = Enumerable.Range(1, faker.Random.Int(5, 20)).Select(_ => new TagEntity { Name = faker.Commerce.ProductName().ToLowerInvariant() }).ToArray();

            _knowledgeBaseDbContext.Tags.AddRange(tags);
            _knowledgeBaseDbContext.SaveChanges();

            var discountCategories = _categories
                .Select((category, index) => new DiscountCategoryEntity
                {
                    Name = category,
                    OrderWeight = index * 100,
                    DiscountCategoryTags = tags.Select(tag => new DiscountCategoryTagEntity { Tag = tag }).ToList(),
                    Discounts = Enumerable.Range(1, faker.Random.Int(3, 10))
                        .Select(number =>
                        {
                            var structure = new[]
                            {
                                new
                                {
                                    type = "p",
                                    children = new[]
                                    {
                                        new { text = faker.Lorem.Paragraph() }
                                    },
                                    id = faker.Random.Int(min: 1, max: int.MaxValue)
                                },
                                new
                                {
                                    type = "p",
                                    children = new[]
                                    {
                                        new { text = "" }
                                    },
                                    id = faker.Random.Int(min: 1, max: int.MaxValue)
                                },
                                new
                                {
                                    type = "p",
                                    children = new[]
                                    {
                                        new { text = faker.Lorem.Paragraph() }
                                    },
                                    id = faker.Random.Int(min: 1, max: int.MaxValue)
                                },
                                new
                                {
                                    type = "p",
                                    children = new[]
                                    {
                                        new { text = "" }
                                    },
                                    id = faker.Random.Int(min: 1, max: int.MaxValue)
                                },
                                new
                                {
                                    type = "p",
                                    children = new[]
                                    {
                                        new { text = faker.Lorem.Paragraph() }
                                    },
                                    id = faker.Random.Int(min: 1, max: int.MaxValue)
                                }
                            };

                            return new DiscountEntity
                            {
                                Name = faker.Commerce.ProductName(),
                                OrderWeight = (index + number * number % 2) * 100,
                                CompanyType = category + " company",
                                Website = faker.Internet.Url(),
                                PhoneNumber = "+xxxxxxxxxxxx",
                                Location = "Minsk",
                                DiscountCities = Enumerable.Range(1, faker.Random.Int(2, discountCities.Length))
                                    .Select(_ => new DiscountCityEntity
                                    {
                                        City = faker.Random.ArrayElement(discountCities)
                                    })
                                    .ToList(),
                                DiscountPercent = faker.Random.Int(2, 20),
                                DiscountTags = Enumerable.Range(1, faker.Random.Int(2, 5))
                                    .Select(_ => new DiscountTagEntity { Tag = faker.Random.ArrayElement(tags) })
                                    .ToList(),
                                DiscountReferences =
                                [
                                    new() { SocialMediaUrl = faker.Internet.Url() },
                                    new() { SocialMediaUrl = faker.Internet.Url() },
                                    new() { SocialMediaUrl = faker.Internet.Url() }
                                ],
                                IsActive = true,
                                Text = JsonSerializer.Serialize(structure),
                                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRmWSR-bANXE5sNDP3dyPJrFlCvku1OQ-gyTg&s",
                            };
                        })
                        .ToList()
                })
                .ToList();

            _knowledgeBaseDbContext.DiscountCategories.AddRange(discountCategories);
            _knowledgeBaseDbContext.SaveChanges();
        }
    }
}
