import { GridColumnWidthsConfiguration } from '@/types/grid';

export const SOFT_SKILLS_TABLE_COLUMNS_WIDTHS: GridColumnWidthsConfiguration = {
	name: {
		width: 242,
	},
	seniority: {
		width: 93,
		fixed: true,
	},
};

export const SPECIALIZATION_COLUMN_NAMES = {
	tab: 'Specialization name',
	section: 'Name',
};

export const SENIORITY_COLUMN_NAME = 'Seniority';

export const SOFT_SKILLS_COLUMN_KEYS = {
	name: 'name',
	seniority: 'seniority',
};

export const SECTION_HEADER = 'Soft skills';
