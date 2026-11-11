import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';

import { FeedbackFormValues } from './types';

export const FEEDBACK_FORM_INITIAL_VALUES: FeedbackFormValues = {
	date: null,
	actions: '',
	conversation: null,
	feedback: '',
	project: null,
	reviewer: '',
	type: null,
};

export const entityName = 'External feedback';
export const formName = 'Teamboard feedback form';

const defaultFeedbackAutoselectProps = {
	valueField: 'name',
	getKey: extractKeyById,
	checkIfOptionEqualToValue: checkIfOptionsEqualById,
};

export const feedbackFormFields = {
	date: {
		innerLabel: 'Date',
		size: 'fullWidth',
	},
	type: {
		label: 'Type',
		...defaultFeedbackAutoselectProps,
	},
	feedback: {
		name: 'feedback',
		label: 'Feedback',
		size: 'fullWidth',
	},
	actions: {
		name: 'actions',
		label: 'Our actions',
		size: 'fullWidth',
	},
	conversation: {
		label: 'Conversation',
		...defaultFeedbackAutoselectProps,
	},
	project: {
		label: 'Project',
		...defaultFeedbackAutoselectProps,
	},
	reviewer: {
		name: 'reviewer',
		label: 'Reviewer',
	},
} as const;

export const conversationQueryParams = {
	skip: 0,
	top: 100000,
};
