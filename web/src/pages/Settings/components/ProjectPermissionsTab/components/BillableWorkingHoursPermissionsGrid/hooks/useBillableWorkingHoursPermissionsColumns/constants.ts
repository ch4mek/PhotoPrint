import { billableWorkingHoursPermissionsColumnsWidths } from '../../constants';

import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';

export const BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS = {
	userId: {
		key: 'userId',
		name: 'Id',
		sortable: true,
		resizable: false,
		width: billableWorkingHoursPermissionsColumnsWidths.id.width,
	},
	userFullName: {
		key: 'userFullName',
		name: 'User',
		sortable: true,
		resizable: false,
		width: billableWorkingHoursPermissionsColumnsWidths.userFullName,
	},
	projectName: {
		key: 'projectName',
		name: 'Project',
		sortable: true,
		resizable: false,
		width: billableWorkingHoursPermissionsColumnsWidths.projectName,
	},
	revokePermission: {
		key: '',
		name: '',
		sortable: false,
		frozen: true,
		headerCellClass: EDIT_COLUMN_HEADER_CLASS,
		cellClass: EDIT_COLUMN_CELL_CLASS,
	},
};
