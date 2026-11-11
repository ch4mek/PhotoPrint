import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PayslipsTable } from '@/components/PayslipsTable';
import { usePayslipsGridState } from '@/components/PayslipsTable/hooks/usePayslipsGridState';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserSalaries } from '@/queries/user/useUserSalaries';

import { PayslipsHeader } from './components/PayslipsHeader';
import {
	RESET_FILTER_COLUMN_INDEX,
	userSalaryGridColumnsKeys,
	useSalaryGridColumnWidth,
} from './constants';
import { useSalaryColumns } from './hooks/useSalaryColumns';
import { useSalaryFiltersReducer } from './hooks/useSalaryFiltersReducer';
import { useSalaryRows } from './hooks/useSalaryRows';
import { PayslipsSectionWrapper, SalaryTabWrapper } from './styles';

export function SalaryTab() {
	const { id } = useAuth();
	const toast = useToast();

	const [filters, setFilters] = useSalaryFiltersReducer();

	const {
		page,
		rowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		setPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: userSalaryGridColumnsKeys.salaryFrom,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserSalaries(
		id,
		rowsPerPage,
		page,
		order,
		orderBy,
		filters,
		toast
	);

	const columns = useSalaryColumns(filters, setFilters);
	const rows = useSalaryRows(data?.items);

	const overrideUserSalaryGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			useSalaryGridColumnWidth,
			true
		);

		return overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, [rows.length]);

	const userSalaryGridPagination = useMemo(
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

	const {
		rows: payslipsRows,
		areFiltersApplied: arePayslipsFiltersApplied,
		filters: payslipsFilters,
		handleSortColumnChange: handlePayslipsSortColumnChange,
		pagination: payslipsPagination,
		resetFilters: resetPayslipsFilters,
		setFilters: setPayslipsFilters,
		sortColumns: sortPayslipsColumns,
		isFetching: isUserPayslipsFetching,
	} = usePayslipsGridState(id);

	const isDataLoading = isLoading || isFetching || isUserPayslipsFetching;

	return (
		<Loader size="small" isLoading={isDataLoading}>
			<SalaryTabWrapper>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...COMMON_GRID_PROPS}
						{...displayMinimalGridRowsProp}
						rows={rows}
						columns={columns}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
						scrollToTopOnRowsChange={false}
						overrideDataGridStyles={overrideUserSalaryGridStyles}
						paginationProps={userSalaryGridPagination}
						hideVerticalScrollbar={
							rows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT && isDataLoading
						}
					/>
				</DataGridHoverRowWrapper>
				<PayslipsSectionWrapper>
					<PayslipsHeader />
					<PayslipsTable
						rows={payslipsRows}
						pagination={payslipsPagination}
						areFiltersApplied={arePayslipsFiltersApplied}
						filters={payslipsFilters}
						filtersSetter={setPayslipsFilters}
						onFiltersReset={resetPayslipsFilters}
						sortColumns={sortPayslipsColumns}
						onSortColumnChange={handlePayslipsSortColumnChange}
					/>
				</PayslipsSectionWrapper>
			</SalaryTabWrapper>
		</Loader>
	);
}
