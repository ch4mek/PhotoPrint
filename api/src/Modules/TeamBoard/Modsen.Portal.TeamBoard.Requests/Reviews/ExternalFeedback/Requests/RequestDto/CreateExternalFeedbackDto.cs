using FluentValidation;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;
internal sealed record CreateExternalFeedbackDto(
    DateOnly ExternalFeedbackDate,
    ExternalFeedbackType ExternalFeedbackType,
    string Feedback,
    string? ActionsTodo,
    string? Reviewer,
    int? ConversationId,
    int ProjectId,
    int UserId)
{
    internal sealed class Validator : AbstractValidator<CreateExternalFeedbackDto>
    {
        public Validator()
        {
            RuleFor(x => x.ExternalFeedbackDate).NotNull();
            RuleFor(x => x.ProjectId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).NotNull().NotEmpty();
            RuleFor(x => x.Feedback).NotNull().NotEmpty();
            RuleFor(x => x.Reviewer)
                .HyphensSpacesRule()
                .LongAlphabeticWithHyphensSpaces();
            RuleFor(x => x.Feedback).LongTextType();
            RuleFor(x => x.ActionsTodo).BeNullOrBeLongTextType();
        }
    }
}
