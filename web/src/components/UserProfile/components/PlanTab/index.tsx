import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { overrideGridStylesWithDynamicCell } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer/styles';
import { Loader } from '@/components/Loader';
import { STATUS_FIELD_NAME } from '@/constants/developmentPlan';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { HeightsContext } from '@/context/CellHeightsContext';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserDevelopmentPlans } from '@/queries/useUserDevelopmentPlans';

import {
	developmentPlanTabTableColumnsWidths,
	EDIT_COLUMN_INDEX,
} from './constants';
import { useDevelopmentPlanColumns } from './hooks/useDevelopmentPlanColumns';
import { useDevelopmentPlanFiltersReducer } from './hooks/useDevelopmentPlanFiltersReducer';
import { useDevelopmentPlanRows } from './hooks/useDevelopmentPlanRows';

export function PlanTab() {
	const { id } = useAuth();

	const [filters, setFilters] = useDevelopmentPlanFiltersReducer();
	const {
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: DEFAULT_ROWS_PER_PAGE_INT,
		initialOrder: SortDirection.ASC,
		initialOrderBy: STATUS_FIELD_NAME,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserDevelopmentPlans({
		userId: id,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		orderBy: getOrderValueForDevelopmentPlans(orderBy, order),
		filters,
	});

	const [rows, setRows] = useDevelopmentPlanRows(data?.items);
	const columns = useDevelopmentPlanColumns(filters, setFilters);

	const { handleUpdateRows, heightsMap, getHeight } = useDynamicRowsHeight({
		setRows,
	});

	const developmentPlanTablePagination = useMemo(
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

	const overrideStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			developmentPlanTabTableColumnsWidths,
			true
		);

		const additionalStylesForEditColumn = overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			EDIT_COLUMN_INDEX,
			gridColumnsTemplate
		);

		return overrideGridStylesWithDynamicCell(additionalStylesForEditColumn);
	}, [rows.length, isLoading]);

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
						overrideDataGridStyles={overrideStyles}
						onRowsChange={handleUpdateRows}
						rowHeight={getHeight}
						scrollToTopOnRowsChange={false}
						paginationProps={developmentPlanTablePagination}
						renderers={noRowsGridFallback}
					/>
				</HeightsContext.Provider>
			</DataGridHoverRowWrapper>
		</Loader>
	);
}
