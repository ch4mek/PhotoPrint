using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.UnitTests;

internal static class TestDataGenerator
{
    internal static FinanceProjectEntity GetFinanceProjectEntity(FinanceClientEntity client, bool isPublic = false, int? id = null)
    {
        return new FinanceProjectEntity
        {
            Id = id ?? 1,
            ProjectName = Guid.CreateVersion7().ToAlphabetic(),
            ClientId = client.Id,
            Client = client,
            Color = "#123412",
            IsActive = true,
            IsPublic = isPublic
        };
    }

    internal static FinanceClientEntity GetFinanceClientEntity(int? id = null, int ? salesManagerId = null)
    {
        return new FinanceClientEntity
        {
            Id = id ?? 1,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            LegalEntity = Guid.CreateVersion7().ToAlphabetic(),
            IsActive = true,
            SalesManagerId = salesManagerId
        };
    }

    internal static FinanceUserEntity GetFinanceUserEntity(int? id = null, string departmentId = "NET", int? resourceGroupId = null)
    {
        return new FinanceUserEntity
        {
            Id = id ?? 1,
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            Email = Guid.CreateVersion7().ToAlphabetic(),
            DepartmentId = departmentId,
            IsActive = true,
            ResourceGroupId = resourceGroupId
        };
    }

    internal static FinanceProjectUserEntity GetFinanceProjectUserEntity(FinanceProjectEntity project,
        FinanceUserEntity user, DateOnly? from = null, DateOnly? to = null, bool isTimeOnProjectBillable = false)
    {
        return new FinanceProjectUserEntity
        {
            Id = 1,
            Project = project,
            ProjectId = project.Id,
            User = user,
            UserId = user.Id,
            UserOnProjectFrom = from ?? new DateOnly(2025, 1, 1),
            UserOnProjectTo = to,
            IsTimeOnProjectBillable = isTimeOnProjectBillable
        };
    }

    internal static DataAccess.Npgsql.Entities.ProjectRateEntity GetProjectRateEntity(FinanceUserEntity? user = null, FinanceProjectEntity? project = null,
        string encrypted = "", string currency = CurrencyCodes.USDollar)
    {
        return new DataAccess.Npgsql.Entities.ProjectRateEntity
        {
            Id = 1,
            UserId = user?.Id ?? 1,
            ProjectId = project?.Id ?? 1,
            HourlyRate = encrypted,
            CurrencyId = currency,    
            RateFrom = new DateOnly(2025, 1, 1),
            User = user,
            Project = project
        };
    }

    internal static ProjectRateEntity GetProjectRateEntity(int id, int userId, int projectId,
        DateOnly rateFrom, DateOnly? rateTo = null, string encrypted = "")
    {
        return new ProjectRateEntity
        {
            Id = id,
            UserId = userId,
            ProjectId = projectId,
            HourlyRate = encrypted,
            CurrencyId = CurrencyCodes.USDollar,
            RateFrom = rateFrom,
            RateTo = rateTo,
        };
    }

    internal static ExpenseCategoryEntity GetExpenseCategoryEntity(int? id = null, string? name = null,
        List<ExpenseEntity>? expenses = null, bool isDeleted = false)
    {
        return new ExpenseCategoryEntity
        {
            Id = id ?? 1,
            Name = name ?? "my test category name",
            OrderWeight = 1,
            IsDeleted = isDeleted,
            Expenses = expenses ?? new List<ExpenseEntity>()
        };
    }

    internal static ExpenseEntity GetExpenseEntity(int categoryId, int id = 1, string expenseAmount = "", int year = 2026, int monthOfYear = 6)
    {
        return new ExpenseEntity
        {
            Id = id,
            ExpenseCategoryId = categoryId,
            Year = year,
            MonthOfYear = monthOfYear,
            IsDeleted = false,
            ExpenseAmount = expenseAmount
        };
    }

    internal static ExpenseEntity GetExpenseEntity(bool isDeleted = false, ExpenseCategoryEntity? expenseCategory = null)
    {
        return new ExpenseEntity
        {
            Id = 1,
            ExpenseCategoryId = 1,
            Year = 2026,
            MonthOfYear = 6,
            ExpenseAmount = "encrypted",
            IsDeleted = isDeleted,
            ExpenseCategory = expenseCategory
        };
    }

    internal static CurrencyEntity GetCurrencyEntity(string code, string? name = null)
    {
        return new CurrencyEntity
        {
            Id = code,
            Name = name ?? Guid.CreateVersion7().ToAlphabetic(),
            CurrencyCode = code,
            CurrencySymbol = null
        };
    }

    internal static List<ExchangeRateEntity> GetExchangeRateEntities(DateOnly date)
    {
        return new List<ExchangeRateEntity>
        {
            new ExchangeRateEntity
            {
                Id = 1,
                FromCurrencyId = CurrencyCodes.PolishZloty,
                FromCurrency = GetCurrencyEntity(CurrencyCodes.PolishZloty),
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ToCurrency = GetCurrencyEntity(CurrencyCodes.BelarusianRuble),
                CurrencyMultiplier = 1,
                Rate = 2.7892m,
                ExchangeDate = date
            },
            new ExchangeRateEntity
            {
                Id = 2,
                FromCurrencyId = CurrencyCodes.RussianRuble,
                FromCurrency = GetCurrencyEntity(CurrencyCodes.RussianRuble),
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ToCurrency = GetCurrencyEntity(CurrencyCodes.BelarusianRuble),
                CurrencyMultiplier = 1,
                Rate = 3.1970m,
                ExchangeDate = date
            },
            new ExchangeRateEntity
            {
                Id = 3,
                FromCurrencyId = CurrencyCodes.USDollar,
                FromCurrency = GetCurrencyEntity(CurrencyCodes.USDollar),
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ToCurrency = GetCurrencyEntity(CurrencyCodes.BelarusianRuble),
                CurrencyMultiplier = 100,
                Rate = 3.7927m,
                ExchangeDate = date
            },
            new ExchangeRateEntity
            {
                Id = 4,
                FromCurrencyId = CurrencyCodes.Euro,
                FromCurrency = GetCurrencyEntity(CurrencyCodes.Euro),
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ToCurrency = GetCurrencyEntity(CurrencyCodes.BelarusianRuble),
                CurrencyMultiplier = 10,
                Rate = 4.1078m,
                ExchangeDate = date
            }
        };
    }

    internal static ExchangeRatesByDateModel GetExchangeRatesByDateModel(DateOnly date)
    {
        return new ExchangeRatesByDateModel
        {
            Date = date,
            ExchangeRates = new List<ExchangeRateModel>
            {
                GetExchangeRateModel(CurrencyCodes.USDollar,Guid.CreateVersion7().ToAlphabetic(), 1, 2.7892m),
                GetExchangeRateModel(CurrencyCodes.Euro, Guid.CreateVersion7().ToAlphabetic(), 1, 3.1970m),
            }
        };
    }

    internal static ExchangeRateModel GetExchangeRateModel(string currencyCode, string currencyName,
        int rateCoef, decimal rateNB)
    {
        return new ExchangeRateModel
        {
            CurrencyCode = currencyCode,
            CurrencyName = currencyName,
            RateCoef = rateCoef,
            RateNB = rateNB
        };
    }

    internal static ProjectUserUpdatedEvent GetProjectUserUpdatedEvent(int projectUserId, int projectId, int userId,
        DateOnly from, DateOnly? to = null)
    {
        return new ProjectUserUpdatedEvent
        {
            EventId = Guid.NewGuid().ToString(),
            ProjectUserId = projectUserId,
            ProjectId = projectId,
            UserId = userId,
            IsTimeOnProjectBillable = true,
            Fte = 1,
            UserOnProjectSeniorityLevelId = "Middle",
            UserOnProjectFrom = from,
            UserOnProjectTo = to
        };
    }
}
