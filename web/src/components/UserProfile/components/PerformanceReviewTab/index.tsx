import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT } from '@/components/Grid/constants';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { overrideGridStylesWithDynamicCell } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer/styles';
import { Loader } from '@/components/Loader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { HeightsContext } from '@/context/CellHeightsContext';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getInfoDateTagContent } from '@/helpers/getInfoDateTagContent';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserPerformanceReview } from '@/queries/teamboard/useUserPerformanceReview';

import {
	DEFAULT_USER_PERFORMANCE_REVIEW_SORT,
	EDIT_COLUMN_INDEX,
	userPerformanceReviewGridColumnWidths,
} from './constants';
import { usePerformanceReviewColumns } from './hooks/usePerformanceReviewColumns';
import { usePerformanceReviewFiltersReducer } from './hooks/usePerformanceReviewFiltersReducer';
import { usePerformanceReviewRows } from './hooks/usePerformanceReviewRows';

export function PerformanceReviewTab() {
	const { id: userId } = useAuth();

	const [filters, setFilters] = usePerformanceReviewFiltersReducer();
	const infoDateTagContent = getInfoDateTagContent();

	const {
		page,
		rowsPerPage,
		order,
		orderBy,
		setPage,
		handleOnPerPageChange,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: DEFAULT_USER_PERFORMANCE_REVIEW_SORT,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserPerformanceReview({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		filters,
		orderBy: `${capitalizeString(orderBy)} ${order}`,
	});

	const columns = usePerformanceReviewColumns(
		filters,
		setFilters,
		infoDateTagContent
	);
	const [rows, setRows] = usePerformanceReviewRows(data?.items);

	const overrideGridStyles = useMemo(() => {
		const gridTemplateColumns = getGridColumnTemplateFromColumnWidths(
			userPerformanceReviewGridColumnWidths,
			true
		);
		const additionalStylesForEditColumn = overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			EDIT_COLUMN_INDEX,
			gridTemplateColumns
		);

		return overrideGridStylesWithDynamicCell(additionalStylesForEditColumn);
	}, [rows.length]);

	const performanceReviewPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	const { handleUpdateRows, getHeight, heightsMap } = useDynamicRowsHeight({
		setRows,
	});

	const isDataLoading = isLoading || isFetching;

	return (
		<Loader isLoading={isDataLoading} size="small">
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<HeightsContext.Provider value={heightsMap}>
					<DataGrid
						rows={rows}
						columns={columns}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						onRowsChange={handleUpdateRows}
						rowHeight={getHeight}
						renderers={noRowsGridFallback}
						scrollToTopOnRowsChange={false}
						overrideDataGridStyles={overrideGridStyles}
						paginationProps={performanceReviewPagination}
						hideVerticalScrollbar={
							rows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT && isDataLoading
						}
					/>
				</HeightsContext.Provider>
			</DataGridHoverRowWrapper>
		</Loader>
	);
}
