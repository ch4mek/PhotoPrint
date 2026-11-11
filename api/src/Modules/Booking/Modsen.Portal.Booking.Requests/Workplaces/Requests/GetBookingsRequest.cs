using FluentValidation;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests;

internal sealed class GetBookingsRequest : IQueryHandler<GetBookingsRequest.Query, BookingAndMeetingRoomsDto>
{
    private readonly IBookingLoader _bookingLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetBookingsRequest(IBookingLoader bookingLoader, IAuthenticatedUserContext authenticatedUserContext)
    {
        _bookingLoader = bookingLoader;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BookingAndMeetingRoomsDto> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsEmployee() && request.IncludeEmployee)
        {
            return _bookingLoader.LoadAsync(request.OfficeId, request.WorkspaceId, cancellationToken);
        }

        var userId = _authenticatedUserContext.GetRequiredUserId();

        if (_authenticatedUserContext.DepartmentId == null && request.UserId != null)
        {
            userId = request.UserId.Value;
        }

        return _bookingLoader.LoadAsync(userId, request.OfficeId, request.WorkspaceId, cancellationToken);
    }

    internal sealed record Query(int? UserId, int? OfficeId, int? WorkspaceId, bool IncludeEmployee)
        : IQuery<BookingAndMeetingRoomsDto>, IBookingRequest;

    internal sealed class GetBookingsRequestValidator : AbstractValidator<Query>
    {
        public GetBookingsRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .When(x => x.UserId is not null);
        }
    }
}
