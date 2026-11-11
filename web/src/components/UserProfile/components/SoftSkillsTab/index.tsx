import { useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { SoftSkillsTable } from '@/components/UserProfile/components/SoftSkillsTab/components/SoftSkillsTable';
import { SOFT_SKILLS_COLUMNS_KEY } from '@/components/UserProfile/components/SoftSkillsTab/constants';
import { useGetAllSnapshotsDates } from '@/components/UserProfile/components/SoftSkillsTab/hooks/useGetAllSnapshotsDates';
import { useSoftSkillsFilters } from '@/components/UserProfile/components/SoftSkillsTab/hooks/useSoftSkillsFilters';
import { useSoftSkillsTableColumns } from '@/components/UserProfile/components/SoftSkillsTab/hooks/useSoftSkillsTableColumns';
import { useSoftSkillsTableRows } from '@/components/UserProfile/components/SoftSkillsTab/hooks/useSoftSkillsTableRows';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserSoftSkills } from '@/queries/softSkills/useUserSoftSkills';

export function SoftSkillsTab() {
	const { id } = useAuth();

	const { filters, setFilters } = useSoftSkillsFilters();

	const {
		rowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
		sortColumns,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: SOFT_SKILLS_COLUMNS_KEY.softSkillId,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserSoftSkills({
		userId: id,
		filters,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		order,
		orderBy,
	});

	const snapshotsDates = useGetAllSnapshotsDates(data?.items);

	const columns = useSoftSkillsTableColumns(
		filters,
		setFilters,
		snapshotsDates,
		data?.items
	);

	const rows = useSoftSkillsTableRows(data?.items);

	const softSkillsTablePagination = useMemo(
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

	const isDataLoading = isLoading || isFetching;

	return (
		<Loader isLoading={isDataLoading} size="small">
			<TabTableWrapper>
				<SoftSkillsTable
					sortColumns={sortColumns}
					rows={rows}
					paginationProps={softSkillsTablePagination}
					columns={columns}
					snapshotsDates={snapshotsDates}
					handleSortColumnChange={handleSortColumnChange}
				/>
			</TabTableWrapper>
		</Loader>
	);
}
