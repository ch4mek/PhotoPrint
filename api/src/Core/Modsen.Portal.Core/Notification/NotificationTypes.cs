using System.ComponentModel;

namespace Modsen.Portal.Core.Notification;

public enum NotificationTypes
{
    [Description("Unknown")]
    None = 0,

    [Description("WorkplaceBookingCancelled")]
    WorkplaceBookingCancelled = 1,

    [Description("TimeTrackReportCreated")]
    TimeTrackReportCreated = 2,

    [Description("TimeTrackReportGenerating")]
    TimeTrackReportGenerating = 3,

    [Description("TimeTrackReportGenerationFailed")]
    TimeTrackReportGenerationFailed = 4,

    [Description("MeetingRoomBookingCancelled")]
    MeetingRoomBookingCancelled = 5,

    [Description("UserInquiryCreated")]
    UserInquiryCreated = 6,

    [Description("DevelopmentPlanSentForReview")]
    DevelopmentPlanSentForReview = 7,

    [Description("DevelopmentPlanReviewed")]
    DevelopmentPlanReviewed = 8,

    [Description("EventRecordsRemoved")]
    EventRecordsRemoved = 9,

    [Description("UserInquiryApproved")]
    UserInquiryApproved = 10,

    [Description("UserInquiryRejected")]
    UserInquiryRejected = 11,

    [Description("UserInquiryCommentCreated")]
    UserInquiryCommentCreated = 12,

    [Description("EmployeeContractExpiring")]
    EmployeeContractExpiring = 13,

    [Description("InquiryTemplateApproversUpdated")]
    InquiryTemplateApproversUpdated = 14
}
