namespace Modsen.Portal.Core.Identity;

public static class AbsencesClaimTypeNames
{
    public const string GroupId = "ABSENCES";

    public static readonly ClaimType ManageAbsenceSettings = new("ManageAbsenceSettings", 100);

    public static readonly ClaimType ViewAbsences = new("ViewAbsences", 200);
}
