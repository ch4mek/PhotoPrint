using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Resolvers;

public class AgreementTransactionsHistoryResolverTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock;
    private readonly AgreementTransactionsHistoryResolver _resolver;

    public AgreementTransactionsHistoryResolverTests()
    {
        _dbContextMock = new Mock<IInquiriesDbContext>();
        _resolver = new AgreementTransactionsHistoryResolver(_dbContextMock.Object);
    }

    [Fact]
    public async Task ResolveAgreementTransactionIdsReturnsCorrectIds()
    {
        // Arrange
        var userId = 1;
        var agreementType = AgreementType.ContractVacationAgreement;
        var startDate = new DateOnly(2025, 10, 1);
        var endDate = new DateOnly(2025, 10, 31);

        var data = new[]
        {
            new UserAgreementTransactionEntity
            {
                Id = 101,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 10),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            },
            new UserAgreementTransactionEntity
            {
                Id = 102,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 15),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            }
        };

        // Используем MockQueryable
        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { 101, 102 }, result);
    }
    [Fact]
    public async Task ResolveAgreementTransactionIdsReturnsCorrectIdsForVacationAgreement()
    {
        // Arrange
        var userId = 1;
        var agreementType = AgreementType.ContractVacationAgreement;
        var startDate = new DateOnly(2025, 10, 1);
        var endDate = new DateOnly(2025, 10, 31);

        var data = new[]
        {
            new UserAgreementTransactionEntity
            {
                Id = 101,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 10),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            },
            new UserAgreementTransactionEntity
            {
                Id = 102,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 15),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            },
            // Другой пользователь
            new UserAgreementTransactionEntity
            {
                Id = 103,
                UserId = 2,
                TransactionDate = new DateTime(2025, 10, 15),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            }
        };

        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { 101, 102 }, result);
    }

    [Fact]
    public async Task ResolveAgreementTransactionIdsReturnsCorrectIdsForSickDaysAgreement()
    {
        // Arrange
        var userId = 1;
        var agreementType = AgreementType.SickDaysAgreement;

        var data = new[]
        {
            new UserAgreementTransactionEntity
            {
                Id = 201,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 12),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.SickDay
                        }
                    }
                }
            },
            new UserAgreementTransactionEntity
            {
                Id = 202,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 18),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            }
        };

        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, null, null, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { 201 }, result); // только SickDay
    }

    [Fact]
    public async Task ResolveAgreementTransactionIdsReturnsEmptyForUnknownAgreementType()
    {
        // Arrange
        var userId = 1;
        var agreementType = (AgreementType)999; // неизвестный тип

        var data = new[]
        {
            new UserAgreementTransactionEntity
            {
                Id = 301,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 12),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            }
        };

        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, null, null, CancellationToken.None);

        // Assert
        Assert.Empty(result); // Unknown type → пустой результат
    }

    [Fact]
    public async Task ResolveAgreementTransactionIdsFiltersByDateRange()
    {
        // Arrange
        var userId = 1;
        var agreementType = AgreementType.ContractVacationAgreement;
        var startDate = new DateOnly(2025, 10, 10);
        var endDate = new DateOnly(2025, 10, 20);

        var data = new[]
        {
            new UserAgreementTransactionEntity
            {
                Id = 401,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 9),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name = "Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            },
            new UserAgreementTransactionEntity
            {
                Id = 402,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 15),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            },
            new UserAgreementTransactionEntity
            {
                Id = 403,
                UserId = userId,
                TransactionDate = new DateTime(2025, 10, 21),
                UserAgreement = new UserAgreementEntity
                {
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                    {
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name="Test",
                            InquiryType = InquiryType.Vacation
                        }
                    }
                }
            }
        };

        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { 402 }, result); // только в диапазоне
    }

    [Fact]
    public async Task ResolveAgreementTransactionIdsReturnsEmptyWhenNoTransactions()
    {
        // Arrange
        var userId = 1;
        var agreementType = AgreementType.ContractVacationAgreement;

        var data = Array.Empty<UserAgreementTransactionEntity>();
        var mockDbSet = data.BuildMockDbSet();
        _dbContextMock.Setup(c => c.UserAgreementTransactions).Returns(mockDbSet.Object);

        // Act
        var result = await _resolver.ResolveAgreementTransactionIds(userId, agreementType, null, null, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
