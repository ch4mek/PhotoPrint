using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;

public record UpdateEquipmentRequestDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string EquipmentTypeId { get; set; }
    public required EquipmentBrokenStatus BrokenStatus { get; set; }
    public EquipmentLocationType Location { get; set; }
    public required string SerialNumber { get; set; }
    public required string InventoryNumber { get; set; }
    public decimal PurchasePrice { get; set; }
    public MoneyCurrencyType PurchaseCurrency { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public string? Comment { get; set; }
    public required string InvoiceNumber { get; set; }
    public required LocationType Storage { get; set; }
    public string? RepairPlace { get; set; }
    public int ApproverId { get; set; }
    public required string JiraTask { get; set; }

    internal sealed class Validator : AbstractValidator<UpdateEquipmentRequestDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.GuaranteeDate)
                .NotNull()
                .DateRule();

            RuleFor(x => x.PurchaseDate)
                .NotNull()
                .DateRule();

            RuleFor(x => x.Name)
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.SerialNumber)
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.InventoryNumber)
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).Money();
            RuleFor(x => x.PurchasePriceUsd).GreaterThanOrEqualTo(0).Money();
            RuleFor(x => x.PurchaseCurrency).IsInEnum();

            RuleFor(x => x.PurchasePlace)
                .NotEmpty()
                .AlphanumericWithSpecialChars()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.Characteristics)
                .LongTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.ApproverId).GreaterThan(0);
            RuleFor(x => x.Location).IsInEnum();

            RuleFor(x => x.Comment)
                .BeEmptyOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.InvoiceNumber)
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.RepairPlace)
                .AlphanumericWithSpecialChars()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.JiraTask)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2)
                .WithMessage("JiraTask must be more than or equal to 2 characters.")
                .MaximumLength(1000)
                .WithMessage("JiraTask must be less than or equal to 1000 characters.");
        }
    }
}
