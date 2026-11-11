import { GridColumnWidthsConfiguration } from '@/types/grid';

const MIN_FULLNAME_COLUMN_WIDTH = 100;
const ABSENCES_COLUMN_WIDTH = 112;

export const TODAY_ABSENCES_COLUMNS = {
	userFullName: 'userFullName',
	absence: 'inquiryTemplateName',
};

export const TODAY_ABSENCES_COLUMNS_WIDTHS: GridColumnWidthsConfiguration<
	typeof TODAY_ABSENCES_COLUMNS
> = {
	userFullName: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	absence: {
		width: ABSENCES_COLUMN_WIDTH,
		fixed: true,
	},
};
