import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const REQUESTS_WIDGET_COLUMNS = {
	userKey: 'userFullName',
	absenceKey: 'inquiryTemplateName',
	dateFromKey: 'dateFrom',
} as const;

export const requestsGridColumnWidths: GridColumnWidthsConfiguration = {
	user: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	absence: {
		width: 110,
		fixed: true,
	},
	dateFrom: {
		width: 100,
		fixed: true,
	},
};
