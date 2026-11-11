using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Notification.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Core;

internal interface INotificationSender
{
    Task SendInquiryAcceptNotification(
        int userId, int inquiryId, IReadOnlyCollection<int> approversIds,
        CancellationToken cancellationToken);

    Task SendInquiryAcceptNotification(
        int userId, UserInquiryCoreDto inquiry, IReadOnlyCollection<int> approversIds,
        CancellationToken cancellationToken);

    Task SendInquiryAcceptNotification(
        string templateName,
        IReadOnlyCollection<InquiryUserNotifyDto> notifyDtos,
        string inquiryType,
        CancellationToken cancellationToken);

    Task SendInquiryApprovedNotification(
        string templateName,
        int userId,
        int inquiryId,
        CancellationToken cancellationToken);

    Task SendInquiryRejectedNotification(
        string templateName,
        int userId,
        int inquiryId,
        CancellationToken cancellationToken);

    Task SendCommentCreatedNotification(
        int userId,
        int inquiryId,
        int commentId,
        IReadOnlyCollection<int> receiversIds,
        CancellationToken cancellationToken);

    Task SendInquiryOrderChangedNotification(
        string templateName,
        IReadOnlyCollection<InquiryUserNotifyDto> notifyDtos,
        InquiryType inquiryType,
        CancellationToken cancellationToken);
}

internal class NotificationSender : INotificationSender
{
    private readonly ICommunicationBus _communicationBus;
    private readonly IUserLoader _userLoader;
    private readonly IUserInquiryLoader _inquiryLoader;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public NotificationSender(
        ICommunicationBus communicationBus,
        IUserLoader userLoader,
        IUserInquiryLoader inquiryLoader,
        IInquiriesDbContext inquiriesDbContext)
    {
        _communicationBus = communicationBus;
        _userLoader = userLoader;
        _inquiryLoader = inquiryLoader;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task SendInquiryAcceptNotification(
        int userId, int inquiryId, IReadOnlyCollection<int> approversIds,
        CancellationToken cancellationToken)
    {
        var inquiry = await _inquiryLoader.Load(inquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");
        await SendInquiryAcceptNotification(userId, inquiry, approversIds, cancellationToken);
    }

    public async Task SendInquiryAcceptNotification(
        int userId, UserInquiryCoreDto inquiry, IReadOnlyCollection<int> approversIds,
        CancellationToken cancellationToken)
    {
        var user = await _userLoader.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User by id {userId} not found");

        var output = new
        {
            Message = $"{inquiry.InquiryTemplateName} request for {user.LastName} {user.FirstName}",
            UserId = userId,
            InquiryId = inquiry.Id,
            inquiry.DateFrom,
            inquiry.DateTo,
            TemplateName = inquiry.InquiryTemplateName,
            inquiry.InquiryType,
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = approversIds,
            NotificationChannel = NotificationChannels.SignalR | NotificationChannels.Teams,
            NotificationType = NotificationTypes.UserInquiryCreated
        };

        await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    public async Task SendInquiryAcceptNotification(
        string templateName,
        IReadOnlyCollection<InquiryUserNotifyDto> notifyDtos,
        string inquiryType,
        CancellationToken cancellationToken)
    {
        var userIds = notifyDtos.Select(x => x.CreatedForUserId!.Value).ToArray();
        var users = await _userLoader.GetUsersByIdsAsync(userIds, cancellationToken);

        var inquiry = await _inquiriesDbContext.UserInquiries
            .Where(x => notifyDtos.Select(n => n.UserInquiryId).Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.DateFrom,
                x.DateTo,
                TemplateName = x.InquiryTemplate!.Name,
                x.InquiryTemplate.InquiryType,
            }).ToListAsync(cancellationToken);

        var notifications = users.Join(
                notifyDtos,
                u => u.Id,
                n => n.CreatedForUserId,
                (u, n) => new
                {
                    n.UserInquiryId,
                    n.AcceptUserId,
                    n.CreatedForUserId,
                    CreatedForFullName = $"{u.LastName} {u.FirstName}"
                })
            .GroupBy(x => x.UserInquiryId)
            .Join(
                inquiry,
                g => g.Key,
                i => i.Id,
                (g, i) => new
                {
                    AcceptUserIds = g.Select(x => x.AcceptUserId),
                    Arguments = new
                    {
                        UserId = g.Select(x => x.CreatedForUserId).First(),
                        InquiryId = g.Key,
                        Message = $"{templateName} request for {g.Select(x => x.CreatedForFullName).First()}",
                        i.DateFrom,
                        i.DateTo,
                        i.TemplateName,
                        InquiryType = i.InquiryType.ToString(),
                    }
                })
            .Select(x => new NotificationsCreatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                Arguments = JsonSerializer.Serialize(x.Arguments),
                UserIds = x.AcceptUserIds.ToArray(),
                NotificationChannel = NotificationChannels.SignalR | NotificationChannels.Teams,
                NotificationType = NotificationTypes.UserInquiryCreated
            })
            .ToArray();

        await _communicationBus.PublishAsync(notifications, cancellationToken);
    }

    public async Task SendInquiryApprovedNotification(
        string templateName,
        int userId,
        int inquiryId,
        CancellationToken cancellationToken)
    {
        _ = await _userLoader.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User by id {userId} not found");
        var inquiry = await _inquiriesDbContext.UserInquiries
            .Where(x => x.Id == inquiryId)
            .Select(x => new
            {
                x.DateFrom,
                x.DateTo,
                TemplateName = x.InquiryTemplate!.Name,
                x.InquiryTemplate.InquiryType,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");

        var output = new
        {
            Message = $"Your {templateName} request has been approved",
            UserId = userId,
            InquiryId = inquiryId,
            inquiry.DateFrom,
            inquiry.DateTo,
            inquiry.TemplateName,
            InquiryType = inquiry.InquiryType.ToString(),
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = [userId],
            NotificationChannel = NotificationChannels.SignalR | NotificationChannels.Teams,
            NotificationType = NotificationTypes.UserInquiryApproved
        };

        await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    public async Task SendInquiryRejectedNotification(
        string templateName,
        int userId,
        int inquiryId,
        CancellationToken cancellationToken)
    {
        _ = await _userLoader.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User by id {userId} not found");
        var inquiry = await _inquiriesDbContext.UserInquiries
            .Where(x => x.Id == inquiryId)
            .Select(x => new
            {
                x.DateFrom,
                x.DateTo,
                TemplateName = x.InquiryTemplate!.Name,
                x.InquiryTemplate.InquiryType,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");

        var output = new
        {
            Message = $"Your {templateName} request has been rejected",
            UserId = userId,
            InquiryId = inquiryId,
            inquiry.DateFrom,
            inquiry.DateTo,
            inquiry.TemplateName,
            InquiryType = inquiry.InquiryType.ToString(),
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = [userId],
            NotificationChannel = NotificationChannels.SignalR | NotificationChannels.Teams,
            NotificationType = NotificationTypes.UserInquiryRejected
        };

        await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    public async Task SendCommentCreatedNotification(
        int userId,
        int inquiryId,
        int commentId,
        IReadOnlyCollection<int> receiversIds,
        CancellationToken cancellationToken)
    {
        var user = await _userLoader.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User by id {userId} not found");
        var inquiry = await _inquiriesDbContext.UserInquiries
            .Where(x => x.Id == inquiryId)
            .Select(x => new
            {
                x.DateFrom,
                x.DateTo,
                TemplateName = x.InquiryTemplate!.Name,
                x.InquiryTemplate.InquiryType,
                Comments = x.UserInquiryComments.Select(f => new { f.Id, f.Comment }),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");

        var output = new
        {
            Message = $"{user.LastName} {user.FirstName} left a comment on {inquiry.TemplateName} request",
            InquiryCommentId = commentId,
            Comment = GetRawComment(inquiry.Comments.FirstOrDefault(x => x.Id == commentId)?.Comment),
            UserId = userId,
            InquiryId = inquiryId,
            inquiry.DateFrom,
            inquiry.DateTo,
            inquiry.TemplateName,
            InquiryType = inquiry.InquiryType.ToString(),
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = receiversIds.Where(x => x != userId).ToArray(),
            NotificationChannel = NotificationChannels.SignalR | NotificationChannels.Teams,
            NotificationType = NotificationTypes.UserInquiryCommentCreated
        };

        await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    public Task SendInquiryOrderChangedNotification(
        string templateName,
        IReadOnlyCollection<InquiryUserNotifyDto> notifyDtos,
        InquiryType inquiryType,
        CancellationToken cancellationToken)
    {
        var notifications = notifyDtos
            .GroupBy(x => x.UserInquiryId)
            .Select(g => new
            {
                AcceptUserIds = g.Select(y => y.AcceptUserId),
                Arguments = new
                {
                    Message = $"Approvers order has been changed for {templateName} request. Please approve or reject it again",
                    InquiryId = g.Key,
                    InquiryType = inquiryType.ToString()
                }
            })
            .Select(x => new NotificationsCreatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                Arguments = JsonSerializer.Serialize(x.Arguments),
                NotificationChannel = NotificationChannels.SignalR,
                UserIds = x.AcceptUserIds.ToArray(),
                NotificationType = NotificationTypes.InquiryTemplateApproversUpdated
            })
            .ToArray();

        return _communicationBus.PublishAsync(notifications, cancellationToken);
    }

    private static string GetRawComment(string? comment)
    {
        if (string.IsNullOrEmpty(comment))
        {
            return string.Empty;
        }

        using var jsonDocument = JsonDocument.Parse(comment);

        var text = jsonDocument.RootElement[0]
            .GetProperty("children")
            .EnumerateArray()
            .Select(x => x.GetProperty("text").GetString() ?? string.Empty)
            .ToList();

        return string.Join("", text);
    }
}
