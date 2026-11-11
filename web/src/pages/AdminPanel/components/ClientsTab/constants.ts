import { defaultIdColumnWidth } from '@/components/Grid/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const CLIENT_PROJECTS_ROW_CLASS_NAME = 'client-projects-cell';
export const CLIENT_ROW_NAME_CLASS_NAME = 'client-cell';

export const clientColumnKeys = {
	id: 'id',
	name: 'name',
	factTime: 'factTrackedTimeInMinutes',
	billableTime: 'billableTrackedTimeInMinutes',
	status: 'isActive',
	registeredLocation: 'registeredLocation',
	comments: 'comments',
};

const {
	id,
	name,
	factTime,
	billableTime,
	status,
	comments,
	registeredLocation,
} = clientColumnKeys;

export const clientColumnsNames = {
	[id]: 'Id',
	[name]: 'Client',
	[factTime]: 'Tracked fact',
	[billableTime]: 'Tracked billable',
	[status]: 'Status',
	[registeredLocation]: 'Reg.loc.',
	[comments]: 'Notes',
};

export const clientColumnsClassNames = {
	[name]: 'client',
	[factTime]: 'tracked-fact',
	[billableTime]: 'tracked-billable',
};

export const CHIEF_ROLES_CLIENT_GRID_WIDTH: GridColumnWidthsConfiguration = {
	id: defaultIdColumnWidth,
	client: {
		width: 240,
		maxWidth: 350,
	},
	factTime: {
		width: 121,
		maxWidth: 137,
	},
	billableTime: {
		width: 137,
		fixed: true,
	},
	status: {
		width: 88,
		maxWidth: 137,
	},
	registeredLocation: {
		width: 99,
		maxWidth: 137,
	},
	notes: 200,
};

export const HEAD_CLIENT_GRID_WIDTH = {
	id: {
		width: 67,
		fixed: true,
	},
	client: {
		width: 240,
		maxWidth: 350,
	},
	factTime: null,
	billableTime: null,
	status: {
		width: 88,
		maxWidth: 137,
	},
	notes: 200,
};

export const CLIENT_EDIT_COLUMN_INDEX = 9;
export const HEAD_CLIENT_EDIT_COLUMN_INDEX = 6;

export enum ClientsGridRowType {
	CLIENT = 'CLIENT',
	PROJECT = 'PROJECT',
}
