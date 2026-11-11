import { ProjectExitFormContentFields } from './types';

export const projectExitFormFields: ProjectExitFormContentFields = {
	user: {
		name: 'user',
		label: 'User',
		valueField: 'name',
	},
	client: {
		name: 'client',
		label: 'Client',
		valueField: 'name',
	},
	project: {
		name: 'project',
		label: 'Project',
		valueField: 'name',
	},
	closingDate: {
		name: 'closingDate',
		label: 'Closing date',
	},
	closingReason: {
		name: 'closingReason',
		label: 'Closing reason',
		valueField: 'name',
	},
} as const;
