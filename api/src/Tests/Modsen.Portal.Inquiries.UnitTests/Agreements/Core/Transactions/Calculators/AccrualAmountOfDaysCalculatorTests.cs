using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Calculators
{
    public class AccrualAmountOfDaysCalculatorTests
    {
        private readonly Mock<IDateCalculator> _mockDateCalculator;
        private readonly AccrualAmountOfDaysCalculator _calculator;

        public AccrualAmountOfDaysCalculatorTests()
        {
            _mockDateCalculator = new Mock<IDateCalculator>();
            _calculator = new AccrualAmountOfDaysCalculator(_mockDateCalculator.Object);
        }

        [Fact]
        public void CalculateWhenAccrualTypeIsOnceInYearAndDateMatchesShouldReturnDaysPerContractYear()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Name = "Test",
                        AccrualType = AccrualType.OnceInYear
                    },
                    DaysPerContractYear = 30
                },
                DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(6))
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now,
                ContractStartDate = DateOnly.FromDateTime(DateTime.Now),
                IsTransactionForYear = false
            };

            // Act
            var result = _calculator.Calculate(agreement, transactionInfo);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public void CalculateWhenAccrualTypeIsMonthlyAndIsTransactionForMonthShouldReturnMonthlyAccrualDays()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Name = "Test",
                        AccrualType = AccrualType.ProportianallyEachMonth
                    },
                    DaysPerContractYear = 25
                },
                DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(6))
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now,
                ContractStartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                IsTransactionForMonth = true,
                IsTransactionForYear = false
            };
            _mockDateCalculator
                .Setup(dc => dc.CalculatePassedMonthsDates(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()).Count)
                .Returns(7);

            // Act
            var result = _calculator.Calculate(agreement, transactionInfo);

            // Assert
            result.Should().Be(2.08m);
        }

        [Fact]
        public void CalculateWhenContractIsIncompleteYearShouldReturnCorrectAccrualDays()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Name = "Test",
                        AccrualType = AccrualType.ProportianallyEachMonth
                    },
                    DaysPerContractYear = 25
                },
                DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(6))
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now,
                ContractStartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                IsTransactionForIncompleteYear = true
            };
            _mockDateCalculator
                .Setup(dc => dc.CalculatePassedMonthsDates(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()).Count)
                .Returns(7);

            // Act
            var result = _calculator.Calculate(agreement, transactionInfo);

            // Assert
            result.Should().Be(2.12m);
        }

        [Fact]
        public void CalculateWhenContractEndsOnTransactionDateShouldReturnCorrectAccrualDays()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Name = "Test",
                        AccrualType = AccrualType.ProportianallyEachMonth
                    },
                    DaysPerContractYear = 25
                },
                DateFrom = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(1))
            };

            var transactionDate = DateTime.Now;

            _mockDateCalculator
                .Setup(dc => dc.CalculatePassedMonthsDates(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()).Count)
                .Returns(11);

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = transactionDate,
                ContractStartDate = DateOnly.FromDateTime(transactionDate.AddMonths(-11)),
                ContractEndDate = DateOnly.FromDateTime(transactionDate),
                IsTransactionForYear = false,
                IsTransactionForMonth = false,
                IsTransactionForIncompleteYear = false
            };

            // Act
            var result = _calculator.Calculate(agreement, transactionInfo);

            // Assert
            result.Should().Be(2.08m);
        }
    }
}
