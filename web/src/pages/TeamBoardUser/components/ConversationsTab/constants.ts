import { ConversationFilter } from '../../types';

export const DEFAULT_CONVERSATION_FILTER_VALUES: ConversationFilter = {
	date: {
		from: null,
		to: null,
	},
	conversationResult: [],
	conversationType: [],
	agreements: '',
	employeeTheses: '',
	theses: '',
	supervisor: '',
};
