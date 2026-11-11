import { fieldsNames } from '../../constants';

export const ON_DISABLED_TOOLTIP_MESSAGE = 'You do not have assigned absences';

export const TEXTAREA_MIN_ROWS = 5;

export const requestFieldData = {
	absenceName: {
		label: 'Absence name',
		valueField: 'name',
	},
	date: {
		innerLabel: 'Date',
	},
	hours: {
		name: fieldsNames.hours,
		currency: 'h',
		label: 'Hours',
	},
	dateRange: {
		innerLabel: 'Date range',
		key: 'dateRange',
	},
	notes: {
		name: fieldsNames.notes,
		label: 'Notes',
	},
};
