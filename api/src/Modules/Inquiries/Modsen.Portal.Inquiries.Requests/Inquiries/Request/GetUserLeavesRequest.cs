using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetUserLeavesRequest : IQueryHandler<GetUserLeavesRequest.Query, List<LeaveDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserLeavesRequest(
        IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<LeaveDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = FilterQuery(request);

        var groupQuery = GroupInquiries(query, request.Top, request.Skip);

        return await groupQuery
            .Select(g => new LeaveDto
            {
                UserId = g.Key!.UserId,
                Name = g.Key.FirstName + ' ' + g.Key.LastName,
                Position = g.Key.Position,
                AvatarFieldId = g.Key.AvatarFileId,
                LeaveDays = g.SelectMany(d => d!.UserInquiryDates
                .Where(date => date.Date.Year == request.Year)
                .Where(date => request.Month == null
                || date.Date.Month == request.Month)
                .Select(t =>
                new LeaveDayDto
                {
                    Date = t.Date,
                    LeaveType = d.InquiryTemplate!.InquiryType.ToString()
                }).ToList()).ToList()
            }).ToListAsync(cancellationToken);
    }

    private IQueryable<UserInquiryEntity> FilterQuery(Query request)
    {
        return _inquiriesDbContext.UserInquiries
            .Where(x => !_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            || x.User!.DepartmentId == _authenticatedUserContext.DepartmentId)
            .Where(x => x.Status == UserInquiryStatus.Approved)
            .Where(x => x.DateFrom.Year == request.Year
            || x.DateTo.Year == request.Year)
            .Where(x => request.Month == null
            || x.DateFrom.Month == request.Month
            || x.DateTo.Month == request.Month)
            .Where(x => request.Position == null || x.User!.Position == request.Position)
            .Include(x => x.UserInquiryDates);
    }

    private static IQueryable<IGrouping<InquiriesUserEntity?, UserInquiryEntity?>> GroupInquiries(
        IQueryable<UserInquiryEntity> query,
        int top,
        int skip)
    {
        return query
            .GroupBy(x => x.User)
            .OrderBy(x => x.Key!.LastName)
            .ThenBy(x => x.Key!.FirstName)
            .Skip(skip)
            .Take(top);
    }

    internal sealed record Query(
    int Top,
    int Skip,
    int Year,
    int? Month,
    string? Position) : IQuery<List<LeaveDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Top)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Skip)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Year)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12);
        }
    }
}
