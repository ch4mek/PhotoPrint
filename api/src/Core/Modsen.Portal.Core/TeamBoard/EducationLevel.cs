using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum EducationLevel
{
    [Description("Higher")]
    Higher,
    [Description("Master's degree")]
    MasterDegree,
    [Description("Secondary specialized")]
    SecondarySpecialized,
    [Description("Technical-vocational")]
    TechnicalVocational,
    [Description("General secondary")]
    GeneralSecondary,
    [Description("Postgraduate+")]
    Postgraduate
}
