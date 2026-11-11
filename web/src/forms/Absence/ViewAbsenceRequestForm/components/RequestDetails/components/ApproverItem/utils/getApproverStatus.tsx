import { Theme } from '@emotion/react';
import { CheckCircleIcon } from '@mms/mms-ui-library/icons/base/CheckCircleIcon';
import { HistoryIcon } from '@mms/mms-ui-library/icons/base/HistoryIcon';
import { NotificationCrossCircleIcon } from '@mms/mms-ui-library/icons/base/NotificationCrossCircleIcon';

import { UserInquiryStatusByApprover } from '@/types/absences';

const { Pending, Accepted, Rejected, WaitingForApprove, Cancelled } =
	UserInquiryStatusByApprover;

export const getApproverStatus = (theme: Theme) => ({
	[Pending]: {
		color: theme.palette['graphics-primary-2'],
		icon: <HistoryIcon />,
	},
	[WaitingForApprove]: {
		color: theme.palette['graphics-primary-2'],
		icon: <HistoryIcon />,
	},
	[Cancelled]: {
		color: theme.palette['graphics-primary-2'],
		icon: <HistoryIcon />,
	},
	[Accepted]: {
		color: theme.palette['feedback-success'],
		icon: <CheckCircleIcon />,
	},
	[Rejected]: {
		color: theme.palette['graphics-error'],
		icon: <NotificationCrossCircleIcon />,
	},
});
