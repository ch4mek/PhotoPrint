import { defaultIdColumnWidth } from '@/components/Grid/constants';
import { fromToDateObject } from '@/constants/defaultValues';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { ProjectExitsGridRow } from '@/types/projectExits';

export const PROJECT_ID_DEFAULT_VALUE = '';
export const DEPARTMENT_FILTER_VALUE_NAME = 'departmentId';
export const GROUP_FILTER_VALUE_NAME = 'userResourceGroupName';
export const CLOSING_REASON_VALUE_NAME = 'closingReasonName';

const flexibleWidthConfig = {
	width: 180,
	fixed: false,
};

export const getProjectExitsColumnsWidths = (
	showDepartment: boolean,
	showGroup: boolean
) => ({
	id: defaultIdColumnWidth,
	user: flexibleWidthConfig,
	...(showDepartment && {
		department: {
			width: 159,
			fixed: true,
		},
	}),
	...(showGroup && {
		group: {
			width: 87,
			fixed: true,
		},
	}),
	project: flexibleWidthConfig,
	client: flexibleWidthConfig,
	closingDate: {
		width: 121,
		fixed: true,
	},
	closingReason: flexibleWidthConfig,
	status: {
		width: 88,
		fixed: true,
	},
});

export const initialProjectExistsFilters = {
	idFilter: PROJECT_ID_DEFAULT_VALUE,
	userFilter: '',
	departmentFilter: [],
	groupFilter: '',
	clientFilter: '',
	projectFilter: '',
	closingDateFilter: fromToDateObject,
	closingReasonFilter: '',
	statusFilter: IS_ACTIVE_DEFAULT_VALUE,
};

export const mockProjectExitsGridRow: ProjectExitsGridRow = {
	id: 0,
	userId: 0,
	fullName: '',
	department: '',
	group: '',
	clientName: '',
	projectName: '',
	closingDate: null,
	closingReason: '',
	isActive: true,
	canViewPersonalInfo: false,
};

export const projectExitGridColumns = {
	id: {
		key: 'id',
		name: 'Id',
	},
	user: {
		key: 'fullName',
		name: 'User',
	},
	department: {
		key: 'department',
		name: 'Department',
	},
	group: {
		key: 'group',
		name: 'Group',
	},
	client: {
		key: 'clientName',
		name: 'Client',
	},
	project: {
		key: 'projectName',
		name: 'Project',
	},
	closingDate: {
		name: 'Closing date',
		key: 'closingDate',
	},
	closingReason: {
		key: 'closingReason',
		name: 'Closing reason',
	},
	status: {
		key: 'isActive',
		name: 'Status',
	},
};

export const projectExitsMenuOptionsValues = {
	edit: 'Edit',
	delete: 'Delete',
	restore: 'Restore',
};

export const DELETE_SUCCESS_MESSAGE =
	'Project exit reason has been successfully deleted';
export const RESTORE_SUCCESS_MESSAGE =
	'Project exit reason has been successfully restored';

export const DELETE_DIALOG_TITLE = 'Project exit';
export const DELETE_DIALOG_CONTENT =
	'Are you sure you want to delete this entry?';

export const RESTORE_DIALOG_TITLE = 'Project exit restoring';
export const RESTORE_DIALOG_CONTENT =
	'Are you sure you want to restore this entry?';
