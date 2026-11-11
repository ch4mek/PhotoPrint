export const FEEDBACK_COLUMNS_KEYS = {
	date: 'externalFeedbackDate',
	type: 'type',
	client: 'client',
	project: 'project',
	feedback: 'feedback',
	actions: 'actions',
	conversation: 'conversation',
	reviewer: 'reviewer',
};

const {
	date,
	type,
	client,
	project,
	feedback,
	actions,
	conversation,
	reviewer,
} = FEEDBACK_COLUMNS_KEYS;

export const FEEDBACK_SECTION_TABLE_WIDTHS_AND_ORDER = {
	[client]: 181,
	[type]: 97,
	[feedback]: 249,
	[actions]: 247,
	[reviewer]: 117,
};

export const FEEDBACK_TAB_TABLE_WIDTHS_AND_ORDER = {
	[date]: 125,
	[type]: 97,
	[client]: 256,
	[project]: 337,
	[feedback]: 332,
	[actions]: 334,
	[conversation]: 122,
	[reviewer]: 178,
};

export const DEFAULT_DATE_FILTER_VALUE = {
	from: null,
	to: null,
};

export const FEEDBACK_COLUMN_NAMES = {
	[date]: 'Date',
	[type]: 'Type',
	[client]: 'Client',
	[project]: 'Project',
	[feedback]: 'Feedback',
	[actions]: 'Our actions',
	[conversation]: 'Conversation',
	[reviewer]: 'Reviewer',
};

export const FEEDBACK_EDIT_COLUMN_OFFSET = 2;
export const FEEDBACK_LINE_HEIGHT = 18;
