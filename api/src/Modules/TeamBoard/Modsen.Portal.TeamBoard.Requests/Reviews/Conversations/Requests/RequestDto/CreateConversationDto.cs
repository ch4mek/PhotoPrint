using FluentValidation;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;
internal record CreateConversationDto(
    DateOnly Date,
    ConversationType ConversationType,
    ConversationResult? ConversationResult,
    string? Theses,
    string? EmployeeTheses,
    string? Agreements,
    DateTime? PerformanceReviewDate,
    int? UserPerformanceReviewId,
    bool? Passed,
    int UserId,
    IReadOnlyCollection<CreateConversationFileDto> Files,
    IReadOnlyCollection<string> DocumentLinks,
    IReadOnlyCollection<UserSalaryFullCoreDto> Salaries,
    IReadOnlyCollection<DevelopmentPlanFullCoreDto> DevelopmentPlans)
{
    internal sealed class CreateConversationCommandValidator : AbstractValidator<CreateConversationDto>
    {
        public CreateConversationCommandValidator()
        {
            RuleFor(x => x.ConversationType).NotNull().NotEmpty();

            RuleFor(x => x.ConversationResult).NotNull().IsInEnum()
                .When(x => x.ConversationType is ConversationType.Conversation or ConversationType.HRConversation);
            RuleFor(x => x.Theses).NotNull().ConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.Conversation);
            RuleFor(x => x.EmployeeTheses).NotNull().ConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.Conversation);
            RuleFor(x => x.Agreements).NotNull().ConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.Conversation);

            RuleFor(x => x.ConversationResult).Null()
                .When(x => x.ConversationType is ConversationType.PerformanceReview);
            RuleFor(x => x.Theses).BeEmptyNullOrBeConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.PerformanceReview);
            RuleFor(x => x.EmployeeTheses).BeEmptyNullOrBeConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.PerformanceReview);
            RuleFor(x => x.Agreements).BeEmptyNullOrBeConversationThesesEmployeesThesesAgreementsTextType().SpacesOrPunctuationSymbolsRule()
                .When(x => x.ConversationType == ConversationType.PerformanceReview);

            RuleFor(x => x.Date).NotNull().NotEmpty().GreaterThanOrEqualTo(new DateOnly(2020, 1, 1));
            RuleFor(x => x.UserId).NotNull().NotEmpty();

            RuleFor(x => x.PerformanceReviewDate)
                .Must(x => x.HasValue && x.Value.Kind == DateTimeKind.Utc)
                .WithMessage("DateTime kind must be Utc")
                .GreaterThanOrEqualTo(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .When(x => x.ConversationType == ConversationType.PerformanceReview);

            RuleFor(x => x.Passed).NotNull().NotEmpty().When(x => x.ConversationType == ConversationType.PerformanceReview);

            RuleFor(x => x.PerformanceReviewDate).Null().Empty().When(x => x.ConversationType != ConversationType.PerformanceReview);
            RuleFor(x => x.Passed).Null().Empty().When(x => x.ConversationType != ConversationType.PerformanceReview);
            RuleFor(x => x.UserPerformanceReviewId).Null().Empty().When(x => x.ConversationType != ConversationType.PerformanceReview);

            RuleFor(x => x.DocumentLinks.Count).LessThanOrEqualTo(5);
            RuleFor(x => x.Files.Count).LessThanOrEqualTo(5);

            RuleFor(x => x.DocumentLinks).ForEach(x => x.Link());

            RuleForEach(x => x.DevelopmentPlans).SetValidator(new UpdateConversationDto.DevelopmentPlanFullCoreDtoValidator());
            RuleForEach(x => x.Salaries).SetValidator(new UpdateConversationDto.SalaryFullCoreDtoValidator());
        }
    }

}
