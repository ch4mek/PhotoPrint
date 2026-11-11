import { Theme } from '@emotion/react';
import { CheckedIcon } from '@mms/mms-ui-library/icons/base/CheckedIcon';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { HistoryIcon } from '@mms/mms-ui-library/icons/base/HistoryIcon';

import { UserInquiryStatusByApprover } from '@/types/absences';

const { Rejected, Accepted, Pending, WaitingForApprove, Cancelled } =
	UserInquiryStatusByApprover;

export const getApproversRequestStatus = (theme: Theme) => ({
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
		icon: <CheckedIcon />,
	},
	[Rejected]: {
		color: theme.palette['graphics-error'],
		icon: <CrossIcon />,
	},
});
