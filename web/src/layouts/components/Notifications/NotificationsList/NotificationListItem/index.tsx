import React, { useMemo } from 'react';

import {
	DevelopmentPlanReviewedNotification,
	DevelopmentPlanSentForReviewNotification,
	EmployeeContractExpiringNotification,
	InquiryApproversUpdatedNotification,
	NotificationType,
	TimeTrackReportCreatedNotification,
	UserNotification,
	UserRequestCommentNotification,
	UserRequestNotification,
	UserRequestApprovedOrRejectedNotification,
} from '@/types/notifications';

import { CreatedTimeTrackReportNotification } from './CreatedTimeTrackReportNotification';
import { EmployeeContractExpiringNotification as EmployeeContractExpiringNotificationComponent } from './EmployeeContractExpiringNotification';
import { GeneratingTimeTrackReportNotification } from './GereratingTimeTrackReportNotification';
import { InquiryApproversUpdatedNotificationComponent } from './InquiryApproversUpdatedNotification';
import { NotificationDissmiss } from './NotificationDissmiss';
import { ReportGeneratingFailureNotification } from './ReportGeneratingFailureNotification';
import { ReviewedDevelopmentPlanNotification } from './ReviewedDevelopmentPlanNotification';
import { SentForReviewDevelopmentPlanNotification } from './SentForReviewDevelopmentPlanNotification';
import {
	NotificationListItemContentWrapper,
	NotificationListItemWrapper,
} from './styles';
import { UserRequestApprovedOrRejectedNotification as UserRequestApprovedOrRejectedNotificationComponent } from './UserRequestApprovedOrRejectedNotification';
import { UserRequestCommentNotificationComponent } from './UserRequestCommentNotificationComponent';
import { UserRequestNotification as UserRequestNotificationComponent } from './UserRequestNotification';

interface NotificationListItemProps {
	item: UserNotification;
}

function NotificationListItemInner({ item }: NotificationListItemProps) {
	const notificationContent = useMemo(() => {
		switch (item.notificationType) {
			case NotificationType.TimeTrackReportGenerationFailed:
				return <ReportGeneratingFailureNotification />;
			case NotificationType.TimeTrackReportCreated:
				return (
					<CreatedTimeTrackReportNotification
						item={item as TimeTrackReportCreatedNotification}
					/>
				);
			case NotificationType.TimeTrackReportGenerating:
				return <GeneratingTimeTrackReportNotification />;
			case NotificationType.DevelopmentPlanReviewed:
				return (
					<ReviewedDevelopmentPlanNotification
						item={item as DevelopmentPlanReviewedNotification}
					/>
				);
			case NotificationType.DevelopmentPlanSentForReview:
				return (
					<SentForReviewDevelopmentPlanNotification
						item={item as DevelopmentPlanSentForReviewNotification}
					/>
				);
			case NotificationType.UserInquiryCreated:
				return (
					<UserRequestNotificationComponent
						item={item as UserRequestNotification}
					/>
				);
			case NotificationType.UserInquiryApproved:
				return (
					<UserRequestApprovedOrRejectedNotificationComponent
						item={item as UserRequestApprovedOrRejectedNotification}
					/>
				);
			case NotificationType.UserInquiryRejected:
				return (
					<UserRequestApprovedOrRejectedNotificationComponent
						item={item as UserRequestApprovedOrRejectedNotification}
					/>
				);
			case NotificationType.UserInquiryCommentCreated:
				return (
					<UserRequestCommentNotificationComponent
						item={item as UserRequestCommentNotification}
					/>
				);
			case NotificationType.InquiryTemplateApproversUpdated:
				return (
					<InquiryApproversUpdatedNotificationComponent
						item={item as InquiryApproversUpdatedNotification}
					/>
				);
			case NotificationType.EmployeeContractExpiring:
				return (
					<EmployeeContractExpiringNotificationComponent
						item={item as EmployeeContractExpiringNotification}
					/>
				);
			default:
				return null;
		}
	}, [item]);

	return (
		<NotificationListItemWrapper>
			<NotificationListItemContentWrapper isRead={item.read}>
				{notificationContent}
			</NotificationListItemContentWrapper>
			<NotificationDissmiss id={item.id} />
		</NotificationListItemWrapper>
	);
}

export const NotificationListItem = React.memo(NotificationListItemInner);
