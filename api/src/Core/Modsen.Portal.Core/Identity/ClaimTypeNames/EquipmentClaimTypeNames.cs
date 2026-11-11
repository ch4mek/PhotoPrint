namespace Modsen.Portal.Core.Identity;

public static class EquipmentClaimTypeNames
{
    public const string GroupId = "EQUIPMENT";

    public const string AvailableEquipmentsWithoutAssigners = "AvailableEquipmentsWithoutAssigners";
    public static readonly ClaimType ManageAllEquipment = new("ManageAllEquipment", 100);
}
