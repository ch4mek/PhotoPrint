import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const EVENTS_WIDGET_COLUMNS = {
	userKey: 'fullName',
	typeKey: 'eventType',
	dateKey: 'eventDate',
} as const;

export const eventsGridColumnWidths: GridColumnWidthsConfiguration = {
	user: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	type: {
		width: 70,
		fixed: true,
	},
	date: {
		width: 100,
		fixed: true,
	},
};

export const EVENT_TYPE_TEXT = {
	birthday: 'Birthday',
	yearSingular: '1 year',
	yearPlural: (years: number) => `${years} years`,
} as const;
