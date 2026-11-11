import { GridColumnWidthsConfiguration } from '@/types/grid';

export const ABSENCE_TABLE_COLUMNS_WIDTHS: GridColumnWidthsConfiguration = {
	name: {
		width: 150,
		fixed: false,
	},
	availableDays: {
		width: 150,
		fixed: false,
	},
};

export const ABSENCE_COLUMNS = {
	name: {
		label: 'Absence Name',
		key: 'name',
	},
	availableDays: {
		label: 'Available days',
		key: 'availableDays',
	},
};
