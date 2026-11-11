import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { UserContractsForm } from '@/forms/TeamboardForm/UserContracts';
import { USER_CONTRACTS_FORM_FIELDS } from '@/forms/TeamboardForm/UserContracts/constants';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { TeamboardUserAccessLevel } from '@/pages/TeamBoardUser/types';
import { hasAccess } from '@/pages/TeamBoardUser/utils';
import { useGetUserContracts } from '@/queries/useGetUserContracts';

import {
	BUTTON_ADD_TITLE,
	CONTRACTS_GRID_COLUMNS_AMOUNT,
	CONTRACTS_GRID_WIDTH,
} from './constants';
import { useContractsColumns } from './hooks/useContractsColumns';
import { useContractsFilters } from './hooks/useContractsFilters';
import { useContractsMutation } from './hooks/useContractsMutation';
import { useContractsRow } from './hooks/useContractsRow';

export function ContractsTab() {
	const toast = useToast();

	const { userId, accessLevel } = useContext(PersonalDataContext);
	const { handleOpen, isOpen, handleClose, id } = useShowModal();

	const { filters, setFilters, areFiltersApplied } = useContractsFilters();

	const {
		page,
		setPage,
		rowsPerPage,
		setRowsPerPage,
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

	const {
		persistentValues: { fireTypes },
	} = useConfiguration();

	const {
		data,
		isFetching: isDataFetching,
		isLoading: isDataLoading,
	} = useGetUserContracts(
		userId!,
		toast,
		page,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		fireTypes.length
	);

	const { createContract, updateContract } = useContractsMutation();

	const handleItemEditClick = useCallback(
		(itemId?: number | string) => () => {
			handleOpen(itemId);
		},
		[]
	);

	const isLoading = isDataFetching || isDataLoading;

	const filterOptions = useMemo(() => ({ fireTypes }), [fireTypes]);

	const canMutateContracts = hasAccess(accessLevel, [
		TeamboardUserAccessLevel.Full,
		TeamboardUserAccessLevel.LimitedHR,
	]);

	const columns = useContractsColumns(
		setFilters,
		filters,
		filterOptions,
		areFiltersApplied,
		canMutateContracts
	);
	const rows = useContractsRow(
		data,
		handleItemEditClick,
		isLoading,
		filterOptions
	);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const contractsGridPagination = useMemo(
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

	const overrideContractsGridStyles = useMemo(() => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			CONTRACTS_GRID_WIDTH,
			true
		);

		return overrideGridStylesWithEditColumn(
			contractsGridPagination ? !isLoading && !!rows.length : false,
			CONTRACTS_GRID_COLUMNS_AMOUNT + 2,
			templateColumns
		);
	}, [contractsGridPagination, rows.length, isLoading]);

	return (
		<TabTableWrapper>
			{canMutateContracts && (
				<TabHeader buttonTitle={BUTTON_ADD_TITLE} onAddClick={handleOpen} />
			)}
			<Loader isLoading={isLoading} size="small">
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
			</Loader>

			{isOpen && (
				<UserContractsForm
					id={Number(id)}
					handleClose={handleClose}
					onCreate={createContract}
					onUpdate={updateContract}
				/>
			)}
		</TabTableWrapper>
	);
}
