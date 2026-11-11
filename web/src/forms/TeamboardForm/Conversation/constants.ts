import {
	CONVERSATION_TYPE_LABELS,
	ConversationFormField,
	ConversationFormValues,
	ConversationType,
	PassedStatus,
} from '@/forms/TeamboardForm/Conversation/types';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { FormAutoselectOption } from '@/types/form';

export const DEFAULT_CONVERSATION_TYPE_OPTIONS: Array<
	FormAutoselectOption<string>
> = [
	{
		id: ConversationType.Conversation,
		name: CONVERSATION_TYPE_LABELS[ConversationType.Conversation],
	},
	{
		id: ConversationType.PerformanceReview,
		name: CONVERSATION_TYPE_LABELS[ConversationType.PerformanceReview],
	},
];

export const CONVERSATION_TYPE_OPTIONS: Array<FormAutoselectOption<string>> = [
	...DEFAULT_CONVERSATION_TYPE_OPTIONS,
	{
		id: ConversationType.HRConversation,
		name: CONVERSATION_TYPE_LABELS[ConversationType.HRConversation],
	},
];

export const PASSED_STATUS_OPTIONS: Array<FormAutoselectOption> = [
	{
		id: 1,
		name: PassedStatus.Yes,
	},
	{
		id: 0,
		name: PassedStatus.No,
	},
];

export const CONVERSATION_FORM_INITIAL_VALUES: ConversationFormValues = {
	agreements: '',
	date: null,
	employeeTheses: '',
	theses: '',
	conversationType: CONVERSATION_TYPE_OPTIONS[0],
	conversationResult: null,
	performanceReviewDate: null,
	passed: null,
	files: [],
	documentLinks: [''],
};

const defaultConversationAutoselectProps = {
	valueField: 'name',
	getKey: extractKeyById,
	checkIfOptionEqualToValue: checkIfOptionsEqualById,
};

export const CONVERSATION_FORM_FIELDS = {
	date: {
		name: ConversationFormField.DATE,
		innerLabel: 'Date',
	},
	conversationType: {
		label: 'Type',
		...defaultConversationAutoselectProps,
	},
	conversationResult: {
		label: 'Result',
		...defaultConversationAutoselectProps,
	},
	performanceReviewDate: {
		name: ConversationFormField.PERFORMANCE_DATE,
		innerLabel: 'Perf. review',
	},
	passed: {
		label: 'Passed',
		...defaultConversationAutoselectProps,
	},
	theses: {
		name: ConversationFormField.THESES,
		label: 'Theses',
	},
	employeeTheses: {
		name: ConversationFormField.EMPLOYEE_THESES,
		label: 'Employee’s theses',
	},
	agreements: {
		name: ConversationFormField.AGREEMENTS,
		label: 'Agreements',
	},
	files: {
		label: '',
		name: ConversationFormField.FILES,
	},
	documentLinks: {
		name: ConversationFormField.DOCUMENT_LINKS,
		label: 'Document link',
	},
};

export const CONVERSATION_TEXT_MAX_LENGTH = 5000;

export const EntityName = 'Conversation';

export const PERFORMANCE_RESPONSE_TYPE = 'Performance review';

export const TOP = 1;
export const SKIP = 0;

export const FORM_NAME = 'Teamboard conversation form';
