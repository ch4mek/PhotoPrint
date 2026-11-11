import { CONVERSATIONS_COLUMNS_NAMES } from '../../constants';

import { ConversationRow } from '@/pages/TeamBoardUser/types';

export const CONVERSATION_VIEW_POPUP_TITLE = 'Conversation details';

export const DEFAULT_VALUE = '-';

export const DETAIL_FIELDS = [
	'date',
	'conversationType',
	'conversationResult',
] as const satisfies ReadonlyArray<keyof ConversationRow>;

const TEXT_SECTION_FIELDS = [
	'theses',
	'employeeTheses',
	'agreements',
] as const satisfies ReadonlyArray<keyof ConversationRow>;

export const getTextSections = (conversation: ConversationRow) =>
	TEXT_SECTION_FIELDS.map((key) => ({
		key,
		label: CONVERSATIONS_COLUMNS_NAMES[key],
		value: conversation[key] as string,
	}));
