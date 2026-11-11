import {
	CONVERSATION_TYPE_OPTIONS,
	DEFAULT_CONVERSATION_TYPE_OPTIONS,
} from '../constants';
import { ConversationType } from '../types';

export const getConversationTypeOptions = (
	hasHRModuleClaim: boolean,
	isIntern?: boolean
) => {
	const options = hasHRModuleClaim
		? CONVERSATION_TYPE_OPTIONS
		: DEFAULT_CONVERSATION_TYPE_OPTIONS;

	if (isIntern) {
		return options.filter(
			({ id }) => id !== ConversationType.PerformanceReview
		);
	}

	return options;
};
