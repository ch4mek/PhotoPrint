import { NotificationType } from '@/types/notifications';

export const notificationsHubEvents = {
	receiveNotification: 'ReceiveNotification',
};

export const NOTIFICATION_LIST_PARENT_CLASS = 'notification-list-container';

export const NOTIFICATION_OPACITY = 0.5;

const {
	TimeTrackReportGenerating,
	TimeTrackReportGenerationFailed,
	WorkplaceBookingCancelled: BookingCancelled,
} = NotificationType;

export const readableNotificationTypes = [
	TimeTrackReportGenerating,
	TimeTrackReportGenerationFailed,
	BookingCancelled,
];

export const NOTIFICATIONS_PER_PAGE = 10;
