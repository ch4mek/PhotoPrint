import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	CONVERSATION_TYPE_LABELS,
	ConversationType,
} from '@/forms/TeamboardForm/Conversation/types';

export const CONVERSATIONS_COLUMNS_KEYS = {
	date: 'date',
	conversationResult: 'conversationResult',
	conversationType: 'conversationType',
	theses: 'theses',
	employeeTheses: 'employeeTheses',
	agreements: 'agreements',
	supervisor: 'supervisor',
	files: 'files',
	documentLinks: 'documentLinks',
};

const {
	date,
	conversationResult,
	conversationType,
	theses,
	employeeTheses,
	agreements,
	supervisor,
	files,
	documentLinks,
} = CONVERSATIONS_COLUMNS_KEYS;

export const CONVERSATIONS_COLUMNS_NAMES = {
	[date]: 'Date',
	[conversationResult]: 'Result',
	[conversationType]: 'Type',
	[theses]: 'Theses',
	[employeeTheses]: 'Employee’s theses',
	[agreements]: 'Agreements',
	[supervisor]: 'Supervisor',
	[files]: 'Documents',
	[documentLinks]: 'Doc. links',
};

export const CONVERSATION_SECTION_TABLE_WIDTHS = {
	date: {
		width: 100,
		fixed: true,
	},
	type: {
		width: 78,
		fixed: true,
	},
	agreements: 309,
};

export const CONVERSATION_TAB_TABLE_WIDTHS = {
	date: {
		width: 100,
		fixed: true,
	},
	conversationResult: {
		width: 88,
		fixed: true,
	},
	conversationType: {
		width: 121,
		fixed: true,
	},
	agreements: 300,
	employeeTheses: 300,
	theses: 300,
	supervisor: {
		width: 200,
		fixed: true,
	},
	files: {
		width: 180,
		fixed: true,
	},
	documentLinks: {
		width: 84,
		fixed: true,
	},
};

export const CONVERSATION_SECTION_EDIT_COLUMN_INDEX = 6;
export const CONVERSATION_TAB_EDIT_COLUMN_INDEX = 11;

export const DEFAULT_DATE_FILTER_VALUE = fromToDateObject;

export const typeOptions: Array<SelectionModalOption> = [
	{
		id: ConversationType.Conversation,
		description: CONVERSATION_TYPE_LABELS[ConversationType.Conversation],
		disabled: false,
	},
	{
		id: ConversationType.PerformanceReview,
		description: CONVERSATION_TYPE_LABELS[ConversationType.PerformanceReview],
		disabled: false,
	},
	{
		id: ConversationType.HRConversation,
		description: CONVERSATION_TYPE_LABELS[ConversationType.HRConversation],
		disabled: false,
	},
];

export const CONVERSATION_LINE_HEIGHT = 18;
