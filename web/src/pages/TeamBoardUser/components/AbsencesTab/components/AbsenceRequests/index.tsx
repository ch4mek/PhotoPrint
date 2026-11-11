import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { Loader } from '@/components/Loader';
import {
	DEFAULT_ABSENCE_REQUEST_EDIT_COLUMN_INDEX,
	requestStatusOptions,
} from '@/constants/absence';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { absenceRequestColumnWidths } from './constants';
import { useAbsenceRequestsColumns } from './hooks/useAbsenceRequestsColumns';
import { GridWrapper } from './styles';
import { AbsenceRequestsProps } from './types';

export function AbsenceRequests({
	filters,
	setFilters,
	absencesOptions,
	rows,
	sortColumns,
	gridPagination,
	handleSortColumnChange,
	handleOpen,
	isPending,
}: AbsenceRequestsProps) {
	const filtersOptions = useMemo(
		() => ({
			absences: absencesOptions.map(({ name }) => ({
				id: name,
				description: name,
			})),
			statuses: requestStatusOptions,
		}),
		[absencesOptions]
	);

	const columns = useAbsenceRequestsColumns(
		filters,
		setFilters,
		filtersOptions,
		handleOpen
	);

	const overrideGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			absenceRequestColumnWidths,
			true,
			undefined,
			true
		);

		return overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			DEFAULT_ABSENCE_REQUEST_EDIT_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, [rows.length]);

	return (
		<GridWrapper defineHeight={false} flexGrow>
			<Loader isLoading={isPending} size="small">
				<DataGrid
					{...{ columns, rows, sortColumns }}
					overrideDataGridStyles={overrideGridStyles}
					rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
					paginationProps={gridPagination}
					renderers={noRowsGridFallback}
					onSortColumnsChange={handleSortColumnChange}
				/>
			</Loader>
		</GridWrapper>
	);
}
