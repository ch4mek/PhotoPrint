import { Roles } from '@/constants/roles';

import { InitUserFilterState } from './types';

export const initUserFilter: InitUserFilterState = {
	id: -1,
	approver: null,
	selectedUser: null,
	issueDate: null,
	returnDate: null,
};

const { HD, GM } = Roles;

const positionDescriptions = {
	[HD]: 'HeadOfDepartment',
	[GM]: 'GroupManager',
} as const;

export const PAGE = 1;
export const ROWS_PER_PAGE = 1;

export const isApproverHD = (positionDescription: string) =>
	positionDescription === positionDescriptions[HD];

export const isApproverGM = (positionDescription: string) =>
	positionDescription === positionDescriptions[GM];

export const FORM_NAME = 'Equipment user form';

export const userFormFields = {
	approver: {
		label: 'Assignment approver',
		name: 'approver',
		valueField: 'fullName',
	},
	user: {
		label: 'User',
		name: 'selectedUser',
		valueField: 'fullName',
	},
	issueDate: {
		innerLabel: 'Issue date',
		name: 'issueDate',
	},
	returnDate: {
		innerLabel: 'Return date',
		name: 'returnDate',
	},
};
