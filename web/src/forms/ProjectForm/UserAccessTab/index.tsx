import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';

import { ProjectAccessForm } from '../ProjectAccessForm';

import { ButtonAdd } from '@/components/ButtonAdd';
import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { SortDirection } from '@/constants/sort';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetUsersByProjectId } from '@/queries/projects/useGetUsersByProjectId';

import {
	USER_ACCESS_TAB_GRID_COLUMNS,
	USER_ACCESS_TAB_GRID_COLUMNS_WIDTH,
	USER_ACCESS_TAB_GRID_COLUMS_AMOUNT,
} from './constants';
import { useUsersAccessFilters } from './hooks/useAccessTabFilters';
import { useGetProjectAccessFormState } from './hooks/useGetProjectAccessFormState';
import { useGetUserAccessTabGridColumns } from './hooks/useGetUserAccessTabGridColumns';
import { useGetUserAccessTabGridRows } from './hooks/useGetUserAccessTabGridRows';
import {
	UserAccessGridWrapper,
	UserAccesssTabHeader,
	UserAccessTabWrapper,
} from './styles';
import { UserAccessTabProps } from './types';

export function UserAccessTab({ projectData }: UserAccessTabProps) {
	const {
		filters,
		setFilters,
		areFiltersAtInitialState,
		isResetFiltersDisabled,
	} = useUsersAccessFilters();

	const {
		rowsPerPage,
		setRowsPerPage,
		order,
		page,
		setPage,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: USER_ACCESS_TAB_GRID_COLUMNS.userOnProjectFrom.key,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useGetUsersByProjectId(
		projectData.id,
		page,
		rowsPerPage,
		order,
		orderBy,
		filters
	);

	const {
		id,
		isOpen,
		isLoading: isProjectAccessFromStateLoading,
		handleFormSubmit,
		handleClose,
		handleOpen,
	} = useGetProjectAccessFormState([
		UsersQueriesKeys.usersByProject,
		`${page}`,
		`${rowsPerPage}`,
		`${projectData.id}`,
		order,
		orderBy,
		JSON.stringify(filters),
	]);

	const { columns } = useGetUserAccessTabGridColumns(
		filters,
		setFilters,
		areFiltersAtInitialState,
		isResetFiltersDisabled
	);

	const showLoader = isLoading || isFetching || isProjectAccessFromStateLoading;

	const handleEditClick = useCallback(
		(projectId?: string | number) => () => {
			handleOpen(Number(projectId));
		},
		[]
	);

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, []);

	const rows = useGetUserAccessTabGridRows(data, handleEditClick);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const userAccessGridPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows, rows.length, page, rowsPerPage, data?.total]
	);

	const overrideUserAccessGridStyles = useMemo(() => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			USER_ACCESS_TAB_GRID_COLUMNS_WIDTH,
			true
		);

		return overrideGridStylesWithEditColumn(
			rows.length > 0,
			USER_ACCESS_TAB_GRID_COLUMS_AMOUNT,
			templateColumns
		);
	}, [rows.length]);

	return (
		<>
			<UserAccessTabWrapper>
				<UserAccesssTabHeader>
					<ButtonAdd onAdd={handleAddClick} />
				</UserAccesssTabHeader>
				<UserAccessGridWrapper>
					<DataGridHoverRowWrapper defineHeight={false}>
						<Loader isLoading={isLoading || isFetching} size="small">
							<DataGrid
								{...COMMON_GRID_PROPS}
								enableVirtualization
								paginationProps={userAccessGridPagination}
								sortColumns={sortColumns}
								onSortColumnsChange={handleSortColumnChange}
								columns={columns}
								rows={rows}
								overrideDataGridStyles={overrideUserAccessGridStyles}
								renderers={noRowsGridFallback}
							/>
						</Loader>
					</DataGridHoverRowWrapper>
				</UserAccessGridWrapper>
			</UserAccessTabWrapper>
			{isOpen && (
				<ProjectAccessForm
					projectAssignId={id ? Number(id) : undefined}
					projectData={projectData}
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					showLoader={showLoader}
				/>
			)}
		</>
	);
}
