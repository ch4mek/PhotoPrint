using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;

internal interface IStudyFormOrderWeightCalculator
{
    int GetOrderWeight(StudyForm formToCalculate);
}

internal class StudyFormOrderWeightCalculator : IStudyFormOrderWeightCalculator
{
    public int GetOrderWeight(StudyForm formToCalculate)
    {
        int result;
        switch (formToCalculate)
        {
            case StudyForm.FullTime:
                result = 500;
                break;

            case StudyForm.Correspodence:
                result = 400;
                break;

            case StudyForm.DistanceLearning:
                result = 300;
                break;

            case StudyForm.Evening:
                result = 200;
                break;

            case StudyForm.BlendingLearning:
                result = 100;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(formToCalculate), formToCalculate, null);
        }

        return result;
    }
}
