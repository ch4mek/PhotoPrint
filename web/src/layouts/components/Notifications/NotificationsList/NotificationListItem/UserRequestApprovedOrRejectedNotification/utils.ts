import { Theme } from '@emotion/react';
import { CheckCircleIcon } from '@mms/mms-ui-library/icons/base/CheckCircleIcon';
import { NotificationCrossCircleIcon } from '@mms/mms-ui-library/icons/base/NotificationCrossCircleIcon';

import { NotificationType } from '@/types/notifications';

export const iconStyles = (theme: Theme) => ({
	[NotificationType.UserInquiryApproved]: {
		Icon: CheckCircleIcon,
		color: theme.palette['feedback-success'],
	},
	[NotificationType.UserInquiryRejected]: {
		Icon: NotificationCrossCircleIcon,
		color: theme.palette['graphics-error'],
	},
});
