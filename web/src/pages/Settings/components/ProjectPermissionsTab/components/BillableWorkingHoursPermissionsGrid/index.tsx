import { useTheme } from '@emotion/react';
import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { DataGridHandle } from 'react-data-grid';

import { overrideProjectPermissionsGridStyles } from '../../grids/styles';
import { useProjectPermissionsFilter } from '../../hooks/useProjectPermissionsFilter';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { Roles } from '@/constants/roles';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getFullName } from '@/helpers/formatName';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUsersLookup } from '@/queries/useUsersLookup';

import { billableWorkingHoursPermissionsColumnsWidths } from './constants';
import { useBillableWorkingHoursPermissions } from './hooks/useBillableWorkingHoursPermissions';
import { useBillableWorkingHoursPermissionsColumns } from './hooks/useBillableWorkingHoursPermissionsColumns';
import { useBillableWorkingHoursPermissionsRows } from './hooks/useBillableWorkingHoursPermissionsRows';
import { BillableWorkingHoursPermissionsRow } from './hooks/useBillableWorkingHoursPermissionsRows/types';

const getRowKey = (row: BillableWorkingHoursPermissionsRow) => row.assignmentId;

export function BillableWorkingHoursPermissionsGrid() {
	const [page, setPage] = useState(0);
	const theme = useTheme();

	const {
		filters: { userFullNameFilter, userIdFilter, projectNameFilter },
		setFilters,
	} = useProjectPermissionsFilter();

	const gridTemplateRef = useRef<DataGridHandle>(null);

	const {
		rowsPerPage,
		setRowsPerPage,
		order,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState({ initialOrderBy: 'userId' });

	const billableWorkingHoursPermissionsRequest =
		useBillableWorkingHoursPermissions({
			page,
			rowsPerPage,
			order,
			orderBy,
			userIdFilter,
			userFullNameFilter,
			projectNameFilter,
		});

	const { isLoading, isFetching } = billableWorkingHoursPermissionsRequest;

	useLoadingStatus(
		BillableWorkingHoursPermissionsGrid.name,
		isFetching || isLoading
	);

	const { data: users } = useUsersLookup({ isActive: true, roles: [Roles.SE] });

	const usersOptions = useMemo(
		() =>
			users
				? users.map(({ firstName, lastName }) =>
						getFullName(firstName, lastName)
				  )
				: [],
		[users]
	);

	const { rows } = useBillableWorkingHoursPermissionsRows({
		billableWorkingHoursPermissionsRequest,
		page,
		rowsPerPage,
	});

	const { columns } = useBillableWorkingHoursPermissionsColumns({
		userFullNameFilter,
		userIdFilter,
		projectNameFilter,
		usersOptions,
		setFilters,
	});

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const overrideGridStyles = useMemo(
		() =>
			overrideProjectPermissionsGridStyles({
				isPaginationShown: !!rows.length,
				templateColumns: getGridColumnTemplateFromColumnWidths(
					billableWorkingHoursPermissionsColumnsWidths,
					true
				),
			}),
		[rows.length]
	);

	const projectGridPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				billableWorkingHoursPermissionsRequest.data?.total
			),
		[
			rows.length,
			page,
			rowsPerPage,
			billableWorkingHoursPermissionsRequest.data?.total,
		]
	);

	useEffect(() => {
		setPage(0);
	}, [userIdFilter, userFullNameFilter, projectNameFilter]);

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				columns={columns}
				rows={rows}
				ref={gridTemplateRef}
				defaultColumnOptions={{
					sortable: true,
				}}
				rowKeyGetter={getRowKey}
				sortColumns={sortColumns}
				onSortColumnsChange={handleSortColumnChange}
				overrideDataGridStyles={overrideGridStyles}
				paginationProps={projectGridPagination}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				stylesConfig={{
					pagination: {
						container: {
							backgroundColor: theme.palette['background-white'],
							borderTop: `1px solid ${theme.palette['graphics-divider-1']}`,
						},
					},
				}}
				{...COMMON_GRID_PROPS}
			/>
		</DataGridHoverRowWrapper>
	);
}
