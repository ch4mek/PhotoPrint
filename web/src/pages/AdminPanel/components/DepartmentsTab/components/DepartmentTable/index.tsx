import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';

import { DepartmentGridRow } from '../../types';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { departmentTableColumnsWidths, HEIGHT_CELL } from './constants';
import { useDepartmentColumns } from './hooks/useDepartmentColumns';
import { useDepartmentGridHover } from './hooks/useDepartmentGridHover';
import { useDepartmentRows } from './hooks/useDepartmentRows';
import {
	DataDepartmentGridHoverRowWrapper,
	overrideDepartmetsTabStyles,
} from './styles';
import { CollapsibleCellWithMenuProps, DepartmentsTableProps } from './types';
import 'react-data-grid/lib/styles.css';

export function DepartmentsTable({ data, menuOptions }: DepartmentsTableProps) {
	const [rows, setRows] = useDepartmentRows(data);

	const departmentColumns = useDepartmentColumns(menuOptions);
	const overrideDepartmentGridStyle = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			departmentTableColumnsWidths
		);

		return overrideDepartmetsTabStyles(gridColumnTemplate);
	}, [rows.length]);

	const handleRowsChangeT = useCallback((newRows: DepartmentGridRow[]) => {
		setRows(newRows);
	}, []);

	const getRowHeight = useMemo(
		() => (row: CollapsibleCellWithMenuProps) =>
			row.departmetItemsRowHeight || HEIGHT_CELL,
		[]
	);
	const { targetRef, handleMouseOver } =
		useDepartmentGridHover<HTMLDivElement>();

	return (
		<DataDepartmentGridHoverRowWrapper
			onMouseOver={handleMouseOver}
			ref={targetRef}
		>
			<DataGrid
				rows={rows}
				rowHeight={getRowHeight}
				columns={departmentColumns}
				overrideDataGridStyles={overrideDepartmentGridStyle}
				renderers={noRowsGridFallback}
				onRowsChange={handleRowsChangeT}
				scrollToTopOnRowsChange={false}
				headerRowHeight={COMMON_GRID_PROPS.headerRowHeight}
			/>
		</DataDepartmentGridHoverRowWrapper>
	);
}
