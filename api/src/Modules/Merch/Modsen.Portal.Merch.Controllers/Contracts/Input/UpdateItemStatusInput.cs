using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Controllers.Contracts.Input;

internal sealed record UpdateItemStatusInput(
    MerchItemStatus Status);
