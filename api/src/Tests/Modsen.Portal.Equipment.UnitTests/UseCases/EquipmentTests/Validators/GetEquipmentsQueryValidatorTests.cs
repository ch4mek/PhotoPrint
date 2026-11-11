using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Validators;

public class GetEquipmentsQueryValidatorTests
{
    private readonly GetEquipmentsRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidQueryOptions))]
    public void ValidateWhenODataQueryIsInvalidShouldReturnInvalidResult(Dictionary<string, string?> queryOptions)
    {
        // Arrange
        var query = new GetEquipmentsRequest.Query(ODataQueryOptionsMock.BuildMock<OdpEquipment>(queryOptions));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidQueryOptions))]
    public void ValidateWhenODataQueryIsValidShouldReturnValidResult(Dictionary<string, string?> queryOptions)
    {
        // Arrange
        var query = new GetEquipmentsRequest.Query(ODataQueryOptionsMock.BuildMock<OdpEquipment>(queryOptions));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static TheoryData<Dictionary<string, string?>> GetInvalidQueryOptions()
    {
        return
        [
            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] =
                    "groupby((id), aggregate(purchasePrice with sum as TotalPrice))",
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = "id mul id as Squared",
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = "true",
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = "005056A2025C1EE2BFE687AFDC2FAAF4_20130807073741",
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = "approver",
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = "verbosejson",
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = "John",
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = "id,type",
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = "id:27",
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = null,
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "10"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "0",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = null
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "0",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "0"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "-1",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "50"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "25",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "50"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "50",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "3"
            }
        ];
    }

    public static TheoryData<Dictionary<string, string?>> GetValidQueryOptions()
    {
        return
        [
            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "0",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "50"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "50",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "50"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = null,
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "1000",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "1"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = "contains(approver/firstName,'Doe')",
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = null,
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "0",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "50"
            },

            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Apply)] = null,
                [nameof(ODataRawQueryOptions.Compute)] = null,
                [nameof(ODataRawQueryOptions.Count)] = null,
                [nameof(ODataRawQueryOptions.DeltaToken)] = null,
                [nameof(ODataRawQueryOptions.Expand)] = null,
                [nameof(ODataRawQueryOptions.Filter)] = "contains(cast(id, Edm.String), '55')",
                [nameof(ODataRawQueryOptions.Format)] = null,
                [nameof(ODataRawQueryOptions.OrderBy)] = "id desc",
                [nameof(ODataRawQueryOptions.Search)] = null,
                [nameof(ODataRawQueryOptions.Select)] = null,
                [nameof(ODataRawQueryOptions.Skip)] = "1",
                [nameof(ODataRawQueryOptions.SkipToken)] = null,
                [nameof(ODataRawQueryOptions.Top)] = "1"
            }
        ];
    }
}
