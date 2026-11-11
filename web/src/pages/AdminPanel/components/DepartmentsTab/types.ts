import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';

import { GridCellDefaultProps } from '@/components/Grid';
import { DepartmentResponse } from '@/types/department';

export interface DepartmentWithGroupManagersFlag extends DepartmentResponse {
	id?: string;
	hasGroupManagers: boolean;
}

export enum DepartmentGridRowType {
	Department,
	Group,
}

export interface DepartmentGridColumns {
	name: string;
	access: string;
}

type BaseDepartmentGridRow =
	| {
			id?: string;
			type?: DepartmentGridRowType.Department;
			showMenu: boolean;
	  }
	| {
			id?: number;
			type?: DepartmentGridRowType.Group;
			showMenu: boolean;
	  };

interface DepartmentGridRowOptions {
	isExpanded?: boolean;
	departmentId?: string;
	groups?: DepartmentGridRow[];
}

export type DepartmentGridRow = GridCellDefaultProps &
	DepartmentGridColumns &
	BaseDepartmentGridRow &
	DepartmentGridRowOptions;

export interface RowsMenuOptions {
	departmentRowMenuOptions: PickerButtonOption[];
	groupRowMenuOptions: PickerButtonOption[];
}
