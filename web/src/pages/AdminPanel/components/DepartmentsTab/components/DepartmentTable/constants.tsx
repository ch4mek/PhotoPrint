import { DepartmentGridColumns } from '../../types';

import {
	GridColumnsNamesConfiguration,
	GridColumnWidthsConfiguration,
} from '@/types/grid';

export const HEIGHT_CELL = 28;

export const DEPARTMENT_PICKER_BUTTON_CLASS = 'department-picker-button';

export const DEPARTMENT_SUBROW_CLASS_NAME = 'department-subrow-item';
export const GROUP_SUBROW_CLASS_NAME = 'group-subrow-item';
export const DEPARTMENT_SUBROW_HOVERED_CLASS_NAME =
	'department-subrow-item-hovered';
export const GROUP_SUBROW_HOVERED_CLASS_NAME = 'group-subrow-item-hovered';
export const GROUP_SUBROW_LAST_CELL_CLASS_NAME =
	'group-subrow-item-hovered-last-cell';

export const departmentsTableColumnsNames: GridColumnsNamesConfiguration<DepartmentGridColumns> =
	{
		name: {
			key: 'name',
			name: 'Department',
		},
		access: {
			key: 'access',
			name: 'Access',
		},
	};

export const departmentTableColumnsWidths: GridColumnWidthsConfiguration<DepartmentGridColumns> =
	{
		name: {
			width: 240,
			fixed: true,
		},
		access: 400,
	};
