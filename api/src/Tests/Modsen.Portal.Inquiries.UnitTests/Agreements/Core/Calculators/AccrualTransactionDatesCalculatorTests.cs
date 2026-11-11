using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Calculators
{
    public class AccrualTransactionDatesCalculatorTests
    {
        private readonly AccrualTransactionDatesCalculator _calculator;

        public AccrualTransactionDatesCalculatorTests()
        {
            _calculator = new AccrualTransactionDatesCalculator();
        }

        [Fact]
        public void CalculateWhenAccrualTypeProportianallyEachMonthShouldReturnMonthlyTransactions()
        {
            // Arrange
            var contractStartDate = new DateOnly(2025, 1, 1);
            var contractEndDate = new DateOnly(2026, 1, 1);
            var transactionDate = new DateOnly(2026, 1, 1);
            var accrualType = AccrualType.ProportianallyEachMonth;

            // Act
            var result = _calculator.Calculate(contractStartDate, contractEndDate, transactionDate, accrualType).ToList();

            // Assert
            result.Count.Should().Be(13);

            for (var month = 1; month <= 11; month++)
            {
                result[month - 1].TransactionDate.Should().Be(new DateTime(2025, month + 1, 1, 0, 0, 0, DateTimeKind.Utc));

                result[month - 1].IsTransactionForMonth.Should().BeTrue();
                result[month - 1].IsTransactionForYear.Should().BeFalse();
                result[month - 1].IsTransactionForIncompleteYear.Should().BeFalse();
            }

            result[11].TransactionDate.Should().Be(new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc));
            result[11].IsTransactionForMonth.Should().BeFalse();
            result[11].IsTransactionForYear.Should().BeFalse();
            result[11].IsTransactionForIncompleteYear.Should().BeTrue();

            result[12].TransactionDate.Should().Be(new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            result[12].IsTransactionForMonth.Should().BeFalse();
            result[12].IsTransactionForYear.Should().BeFalse();
            result[12].IsTransactionForIncompleteYear.Should().BeFalse();
        }

        [Fact]
        public void CalculateWhenAccrualTypeOnceInYearShouldReturnYearlyTransactions()
        {
            // Arrange
            var contractStartDate = new DateOnly(2022, 1, 1);
            var contractEndDate = new DateOnly(2026, 1, 1);
            var transactionDate = new DateOnly(2026, 1, 1);
            var accrualType = AccrualType.OnceInYear;

            // Act
            var result = _calculator.Calculate(contractStartDate, contractEndDate, transactionDate, accrualType).ToList();

            // Assert
            result.Count.Should().Be(5);

            for (var year = 0; year < 5; year++)
            {
                result[year].TransactionDate.Should()
                    .Be(new DateTime(2022 + year, 1, 1, 0, 0, 0, DateTimeKind.Utc));

                result[year].IsTransactionForMonth.Should().BeTrue();
                result[year].IsTransactionForYear.Should().BeTrue();
                result[year].IsTransactionForIncompleteYear.Should().BeFalse();
            }
        }
    }
}
