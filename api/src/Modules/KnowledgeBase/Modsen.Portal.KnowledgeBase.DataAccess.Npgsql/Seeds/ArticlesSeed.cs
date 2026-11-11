using System.Text.Json;
using Bogus;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Seeds;

internal class ArticlesSeed : ISeedEntitiesProvider<KnowledgeBaseDbContext>
{
    private readonly KnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public ArticlesSeed(KnowledgeBaseDbContext knowledgeBaseDbContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsDevelopmentOrQaEnvironment() &&
            !_knowledgeBaseDbContext.Articles.Any())
        {
            var tags = _knowledgeBaseDbContext.Tags.ToArray();
            var faker = new Faker();
            var articleFaker = new Faker<ArticleEntity>()
                .RuleFor(x => x.Name, fake => fake.Lorem.Sentence(wordCount: 5))
                .RuleFor(x => x.IsActive, true)
                .RuleFor(x => x.OrderWeight, fake => fake.Random.Int(0, 1000))
                .RuleFor(x => x.Text, fake =>
                {
                    var structure = new[]
                    {
                        new
                        {
                            type = "h1",
                            children = new[]
                            {
                                new { text = "" }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = "" }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = fake.Lorem.Paragraph() }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = "" }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = fake.Lorem.Paragraph() }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = "" }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        },
                        new
                        {
                            type = "p",
                            children = new[]
                            {
                                new { text = fake.Lorem.Paragraph() }
                            },
                            id = fake.Random.Int(min: 1, max: int.MaxValue)
                        }
                    };

                    return JsonSerializer.Serialize(structure);
                })
                .RuleFor(x => x.ArticleTags, fake => Enumerable.Range(1, fake.Random.Int(1, 5))
                    .Select(_ => new ArticleTagEntity { Tag = fake.Random.ArrayElement(tags), })
                    .ToList())
                .RuleFor(x => x.IsAvailableOnMainScreen, fake => fake.Random.Bool(0.4f))
                .RuleFor(x => x.Views, 0)
                .RuleFor(x => x.Keywords, fake => fake.Commerce.ProductName());

            var groups = new GroupEntity[]
            {
                new()
                {
                    Name = "General",
                    OrderWeight = 0,
                    IsActive = true,
                    IsAvailableOnMainScreen = true,
                    Articles = articleFaker.Generate(count: faker.Random.Int(3, 7))
                },
                new()
                {
                    Name = "Events",
                    OrderWeight = 100,
                    IsActive = true,
                    IsAvailableOnMainScreen = true,
                    Articles = articleFaker.Generate(count: faker.Random.Int(1, 2)),
                    ChildGroups =
                    [
                        new GroupEntity
                        {
                            Name = "Meetups",
                            OrderWeight = 0,
                            IsActive = true,
                            IsAvailableOnMainScreen = true,
                            Articles = articleFaker.Generate(count: faker.Random.Int(3, 7))
                        },

                        new GroupEntity()
                        {
                            Name = "Summer Corporates",
                            OrderWeight = 100,
                            IsActive = true,
                            IsAvailableOnMainScreen = false,
                            Articles = articleFaker.Generate(count: faker.Random.Int(3, 7))
                        }
                    ]
                },
                new()
                {
                    Name = "Instructions",
                    OrderWeight = 200,
                    IsActive = true,
                    IsAvailableOnMainScreen = true,
                    Articles = articleFaker.Generate(count: faker.Random.Int(3, 7))
                }
            };

            _knowledgeBaseDbContext.Groups.AddRange(groups);
            _knowledgeBaseDbContext.SaveChanges();
        }
    }
};
