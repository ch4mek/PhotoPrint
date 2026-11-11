using FluentValidation;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

internal record UpdateConversationDto(
    int Id,
    ConversationType ConversationType,
    ConversationResult? ConversationResult,
    string? Theses,
    string? EmployeeTheses,
    string? Agreements,
    DateTime? PerformanceReviewDate,
    bool? Passed,
    int UserId,
    IReadOnlyCollection<CreateConversationFileDto> Files,
    IReadOnlyCollection<string> DocumentLinks,
    IReadOnlyCollection<UserSalaryFullCoreDto> Salaries,
    IReadOnlyCollection<DevelopmentPlanFullCoreDto> DevelopmentPlans)
{
    internal sealed class UpdateConversationCommandValidator : AbstractValidator<UpdateConversationDto>
    {
        public UpdateConversationCommandValidator()
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

            RuleFor(x => x.UserId).NotNull().NotEmpty();
            RuleFor(x => x.PerformanceReviewDate).NotNull().NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .When(x => x.ConversationType == ConversationType.PerformanceReview);
            RuleFor(x => x.Passed).NotNull().NotEmpty().When(x => x.ConversationType == ConversationType.PerformanceReview);

            RuleFor(x => x.DocumentLinks.Count).LessThanOrEqualTo(5);
            RuleFor(x => x.Files.Count).LessThanOrEqualTo(5);

            RuleFor(x => x.DocumentLinks).ForEach(x => x.Link());

            RuleForEach(x => x.DevelopmentPlans).SetValidator(new DevelopmentPlanFullCoreDtoValidator());
            RuleForEach(x => x.Salaries).SetValidator(new SalaryFullCoreDtoValidator());
        }
    }

    internal sealed class SalaryFullCoreDtoValidator : AbstractValidator<UserSalaryFullCoreDto>
    {
        public SalaryFullCoreDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage("This field is required");

            RuleFor(x => x.SalaryUsd)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Salary is required if provided.")
                .ExclusiveBetween(0m, 10_000_000m)
                .WithMessage("Salary must be exclusive between 0 and 10,000,000.");

            RuleFor(x => x.SalaryFrom)
                .GreaterThanOrEqualTo(new DateOnly(2020, 1, 1))
                .WithMessage("Date must be greater than or equal to 01.01.2020.");
        }
    }

    internal sealed class DevelopmentPlanFullCoreDtoValidator : AbstractValidator<DevelopmentPlanFullCoreDto>
    {
        public DevelopmentPlanFullCoreDtoValidator()
        {
            RuleFor(x => x.Summary).NotNull().NotEmpty()
                .LongTextType()
                .SpacesOrPunctuationSymbolsRule();
            RuleFor(x => x.Notes)
                .BeEmptyNullOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();
        }
    }
}
