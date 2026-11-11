import { ManageRequestType } from '@/types/absences';

import { MessageLabelByType } from './types';

const { Accept, Reject } = ManageRequestType;

export const messageLabelByType = {
	[Accept]: {
		toast: 'approved',
		title: 'approving',
	},
	[Reject]: {
		toast: 'rejected',
		title: 'rejecting',
	},
} satisfies MessageLabelByType;
