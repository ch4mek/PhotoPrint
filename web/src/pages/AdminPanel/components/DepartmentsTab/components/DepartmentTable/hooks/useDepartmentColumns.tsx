import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { DepartmentGridRow, RowsMenuOptions } from '../../../types';
import { DepartmentAccessCell } from '../components/DepartmentAccessCell';
import { DepartmentCellRenderer } from '../components/DepartmentCellRenderer';
import {
	departmentTableColumnsWidths,
	departmentsTableColumnsNames,
} from '../constants';

import { getColumnFactory } from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';

export function useDepartmentColumns(menuOptions: RowsMenuOptions) {
	const headers: Column<DepartmentGridRow>[] = useMemo(
		() => [
			getColumnFactory({
				...departmentsTableColumnsNames.name,
				width: getColumnWidth(departmentTableColumnsWidths.name).width,

				renderCell: (props) => (
					<DepartmentCellRenderer menuOptions={menuOptions} {...props} />
				),
			}),
			getColumnFactory({
				...departmentsTableColumnsNames.access,
				width: getColumnWidth(departmentTableColumnsWidths.access).width,

				renderCell: DepartmentAccessCell,
			}),
		],
		[]
	);

	return headers;
}
