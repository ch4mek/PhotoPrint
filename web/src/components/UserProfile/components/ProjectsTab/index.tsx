import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import {
	convertUserProjectsToRows,
	getRowKey,
} from '@/components/UserProfile/utils';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { isRegularEmployee } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useProjectsFilters } from '@/hooks/useProjectsFilters';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserProjectAssignments } from '@/queries/useUserProjectAssignments';

import {
	columnsWidth,
	columnsWidthForEmployee,
	defaultFilterValues,
	projectTableColumns,
	RESET_FILTER_COLUMN_INDEX,
} from './constants';
import { useProjectTableColumns } from './hooks/useProjectTableColumns';

export function ProjectsTab() {
	const { role, id } = useAuth();
	const isUserEmployee = isRegularEmployee(role);
	const { filters, setFilters, areFiltersApplied, resetFilters } =
		useProjectsFilters(defaultFilterValues);

	const {
		rowsPerPage,
		handleOnPerPageChange,
		page,
		setPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: projectTableColumns.startDate.key,
		resetPageDeps: [filters],
	});

	const actualColumnsWidth = isUserEmployee
		? columnsWidthForEmployee
		: columnsWidth;

	const { data, isLoading, isFetching } = useUserProjectAssignments(
		id,
		page,
		rowsPerPage,
		filters,
		order,
		orderBy
	);

	const columns = useProjectTableColumns(
		actualColumnsWidth,
		filters,
		isUserEmployee,
		setFilters,
		areFiltersApplied,
		resetFilters
	);

	const rows = useMemo(() => {
		if (!data) {
			return [];
		}

		return convertUserProjectsToRows(data.items);
	}, [data]);

	const pagination = useMemo(
		() =>
			createGridPagination(
				data?.items.length || 0,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[data?.items.length, page, rowsPerPage, data?.total]
	);

	const overrideGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			actualColumnsWidth,
			true
		);

		return overrideGridStylesWithEditColumn(
			pagination ? !isLoading && !!rows.length : false,
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, [rows.length, pagination, isLoading, actualColumnsWidth]);

	return (
		<Loader isLoading={isLoading || isFetching} size="small">
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					{...COMMON_GRID_PROPS}
					enableVirtualization
					columns={columns}
					rows={rows}
					rowKeyGetter={getRowKey}
					renderers={noRowsGridFallback}
					overrideDataGridStyles={overrideGridStyles}
					scrollToTopOnRowsChange={false}
					paginationProps={pagination}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
				/>
			</DataGridHoverRowWrapper>
		</Loader>
	);
}
