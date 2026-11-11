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

export const SOFT_SKILLS_COLUMN_KEYS = {
	name: 'name',
	seniority: 'seniority',
};
