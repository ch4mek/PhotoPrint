import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useContext, useEffect, useMemo } from 'react';

import { GroupInfoContext } from '../context/GroupInfoContext';
import { useGroupMembersColumns } from '../hooks/useGroupMembersColumns';
import { useGroupMembersValues } from '../hooks/useGroupMembersValues';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useConfiguration } from '@/context/Configuration';
import { createGridPagination } from '@/helpers/createGridPagination';
import { DEFAULT_USER_ORDER_BY } from '@/pages/AdminPanel/components/UsersTab/constants';
import { useTableState } from '@/pages/hooks/useTableState';
import { GroupUsersContract } from '@/queries/groups/types';
import { useGetGroupUsers } from '@/queries/groups/useGetGroupUsers';
import { useGroupUsersLookup } from '@/queries/groups/useGroupUsersLookup';

import {
	CLEAR_FILTERS_COLUMN_INDEX,
	GROUP_MEMBERS_TABLE_COLUMN_WIDTHS,
} from './constants';
import { FilterOptions, GroupMembersTableProps } from './types';

export function GroupMembersTable({
	displayFilters = true,
	filters,
	filterSetters,
	isColumnSortable = true,
	onGroupMembersFetching,
}: GroupMembersTableProps) {
	const {
		persistentValues: { locationTypes },
	} = useConfiguration();

	const { groupId } = useContext(GroupInfoContext);

	const {
		page,
		setPage,
		rowsPerPage,
		sortColumns,
		handleSortColumnChange,
		order,
		orderBy,
		handleOnPerPageChange,
	} = useTableState({
		initialOrderBy: DEFAULT_USER_ORDER_BY,
		resetPageDeps: [filters],
	});

	const { data, isFetching } = useGetGroupUsers({
		groupId,
		rowsPerPage,
		page,
		filters,
		allLocations: locationTypes,
		order,
		orderBy,
	});

	const { data: usersLookup } = useGroupUsersLookup({ groupId });

	useEffect(() => {
		onGroupMembersFetching?.(isFetching);
	}, [onGroupMembersFetching, isFetching]);

	const filterOptions = useMemo<FilterOptions>(
		() => ({
			location: [...(locationTypes || []), NO_VALUE_FILTER_OPTION],
			users: usersLookup?.map(({ fullName }) => fullName) ?? [],
		}),
		[usersLookup, isFetching, locationTypes]
	);

	const columns = useGroupMembersColumns(
		displayFilters,
		GROUP_MEMBERS_TABLE_COLUMN_WIDTHS,
		filterOptions,
		filters,
		filterSetters,
		isColumnSortable
	);

	const overrideGroupMembersGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			GROUP_MEMBERS_TABLE_COLUMN_WIDTHS,
			true
		);

		return overrideGridStylesWithEditColumn(
			Boolean(data?.items.length),
			CLEAR_FILTERS_COLUMN_INDEX,
			gridColumnTemplate
		);
	}, [data]);

	const groupMembersPagination = useMemo(
		() =>
			createGridPagination(
				data?.items.length ?? 0,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[data?.items, page, rowsPerPage, data?.total]
	);

	const displayedGridData: Array<GroupUsersContract> = useMemo(
		() => (data && !isFetching ? data.items : []),
		[data?.items, isFetching]
	);

	const rows = useGroupMembersValues(displayedGridData);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<Loader isLoading={isFetching}>
				<DataGrid
					{...COMMON_GRID_PROPS}
					columns={columns}
					rows={rows}
					overrideDataGridStyles={overrideGroupMembersGridStyles}
					renderers={noRowsGridFallback}
					enableVirtualization
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					paginationProps={groupMembersPagination}
				/>
			</Loader>
		</DataGridHoverRowWrapper>
	);
}
