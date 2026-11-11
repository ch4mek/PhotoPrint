using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;

internal interface IEducationLevelOrderWeightCalculator
{
    int GetOrderWeight(EducationLevel educationToCalculate);
}

internal class EducationLevelOrderWeightCalculator : IEducationLevelOrderWeightCalculator
{
    public int GetOrderWeight(EducationLevel educationToCalculate)
    {
        int result;
        switch (educationToCalculate)
        {
            case EducationLevel.Higher:
                result = 600;
                break;

            case EducationLevel.MasterDegree:
                result = 500;
                break;

            case EducationLevel.SecondarySpecialized:
                result = 400;
                break;

            case EducationLevel.TechnicalVocational:
                result = 300;
                break;

            case EducationLevel.GeneralSecondary:
                result = 200;
                break;

            case EducationLevel.Postgraduate:
                result = 100;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(educationToCalculate), educationToCalculate, null);
        }

        return result;
    }
}
