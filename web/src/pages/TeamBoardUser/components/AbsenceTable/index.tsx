import { DataGrid } from '@mms/mms-ui-library/components/Grids';

import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { useAbsenceTableColumns } from './hooks/useAbsenceTableColumns';
import { overrideAbsenceSectionTableStyles } from './styles';
import { AbsenceTableProps } from './types';
import { getRowKey } from './utils';

export function AbsenceTable({ rows }: AbsenceTableProps) {
	const columns = useAbsenceTableColumns();

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				{...COMMON_GRID_PROPS}
				{...displayMinimalGridRowsProp}
				enableVirtualization
				columns={columns}
				rows={rows}
				rowKeyGetter={getRowKey}
				overrideDataGridStyles={overrideAbsenceSectionTableStyles}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				maxRowsCount={MAX_VISIBLE_ROWS_COUNT}
			/>
		</DataGridHoverRowWrapper>
	);
}
