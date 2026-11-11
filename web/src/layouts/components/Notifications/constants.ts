import { CSSProperties } from 'react';

import { NotificationType } from '@/types/notifications';

const NOTIFICATIONS_POPPER_WIDTH = 400;
const NOTITFICATIONS_POPPER_SIDE_PADDING = 16;
const CALCULATED_NOTIFICATIONS_POPPER_WIDTH =
	NOTIFICATIONS_POPPER_WIDTH + NOTITFICATIONS_POPPER_SIDE_PADDING * 2;

export const popperStyles: CSSProperties = {
	maxWidth: `${CALCULATED_NOTIFICATIONS_POPPER_WIDTH}px`,
	padding: `0 ${NOTITFICATIONS_POPPER_SIDE_PADDING}px`,
	width: '100%',
};

export const popperOffset: [number, number] = [20, 16];
export const POPPER_BOTTOM_OFFSET = 24;

export const HEADER_INCLUDED_NOTIFCATION_TYPES = [
	NotificationType.TimeTrackReportCreated,
	NotificationType.TimeTrackReportGenerating,
	NotificationType.TimeTrackReportGenerationFailed,
	NotificationType.DevelopmentPlanReviewed,
	NotificationType.DevelopmentPlanSentForReview,
	NotificationType.UserInquiryCreated,
	NotificationType.EmployeeContractExpiring,
	NotificationType.UserInquiryApproved,
	NotificationType.UserInquiryRejected,
	NotificationType.UserInquiryCommentCreated,
	NotificationType.InquiryTemplateApproversUpdated,
];

export const defaultReconnectMilliseconds = [0, 2000, 10000, 30000];
