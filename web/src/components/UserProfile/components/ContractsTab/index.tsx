import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { fromToDateObject } from '@/constants/defaultValues';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useToast } from '@/context/Toast';
import { USER_CONTRACTS_FORM_FIELDS } from '@/forms/TeamboardForm/UserContracts/constants';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetUserContracts } from '@/queries/useGetUserContracts';

import { CONTRACTS_GRID_COLUMNS_AMOUNT, contractsGridWidth } from './constants';
import { useContractsColumns } from './hooks/useContractsColumns';
import { useContractsFilters } from './hooks/useContractsFilters';
import { useContractsRow } from './hooks/useContractsRow';

export function ContractsTab() {
	const toast = useToast();

	const { userId } = useContext(PersonalDataContext);

	const { filters, setFilters, areFiltersApplied } = useContractsFilters();

	const {
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: USER_CONTRACTS_FORM_FIELDS.contractFrom.name,
		resetPageDeps: [filters],
	});

	const filtersWithDefaultFireFilters = useMemo(
		() => ({
			...filters,
			fireDate: fromToDateObject,
			fireType: [],
			fireReason: '',
			fireDescription: '',
		}),
		[filters]
	);

	const { data, isFetching, isLoading } = useGetUserContracts(
		userId!,
		toast,
		page,
		rowsPerPage,
		order,
		orderBy,
		filtersWithDefaultFireFilters
	);

	const isDataLoading = isFetching || isLoading;

	const columns = useContractsColumns(setFilters, filters, areFiltersApplied);
	const rows = useContractsRow(data);

	const contractsGridPagination = useMemo(
		() =>
			createGridPagination(
				rows.length || 0,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	const overrideContractsGridStyles = useMemo(() => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			contractsGridWidth,
			true
		);

		const amountContractsGrid = CONTRACTS_GRID_COLUMNS_AMOUNT + 2;

		return overrideGridStylesWithEditColumn(
			contractsGridPagination ? !isLoading && !!rows.length : false,
			amountContractsGrid,
			templateColumns
		);
	}, [contractsGridPagination, rows.length, isLoading]);

	return (
		<Loader isLoading={isDataLoading} size="small">
			<TabTableWrapper>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						overrideDataGridStyles={overrideContractsGridStyles}
						columns={columns}
						rows={rows}
						paginationProps={contractsGridPagination}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
					/>
				</DataGridHoverRowWrapper>
			</TabTableWrapper>
		</Loader>
	);
}
