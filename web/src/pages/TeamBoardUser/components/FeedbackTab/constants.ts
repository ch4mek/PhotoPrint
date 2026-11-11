import { FeedbackFilter } from '../../types';

import { fromToDateObject } from '@/constants/defaultValues';

export const DEFAULT_FEEDBACK_FILTER_VALUES: FeedbackFilter = {
	date: fromToDateObject,
	actions: '',
	client: '',
	feedback: '',
	project: '',
	reviewer: '',
	type: [],
};
