import { useEffect, useMemo, useState } from 'react';

import { Loader } from '../Loader';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { TabHeader } from '@/components/TabHeader';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { SortDirection } from '@/constants/sort';
import { TeamboardUserProjectsForm } from '@/forms/TeamboardForm/UserProject';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useProjectsFilters } from '@/hooks/useProjectsFilters';
import { useTableState } from '@/pages/hooks/useTableState';
import { ProjectsTable } from '@/pages/TeamBoardUser/components/ProjectsTable';
import { PROJECT_TABLE_COLUMNS } from '@/pages/TeamBoardUser/components/ProjectsTable/constants';
import {
	convertUserProjectLookupToRows,
	getProjectTableLoaderRows,
} from '@/pages/TeamBoardUser/components/TeamBoardUserSummaryTab/ProjectSection/utils';
import { useUserProjectAssignmentFormState } from '@/pages/TeamBoardUser/hooks/useUserProjectAssingmentFormState';
import { ProjectRow } from '@/pages/TeamBoardUser/types';
import { useUserProjectAssignments } from '@/queries/useUserProjectAssignments';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	DEFAULT_USER_PROJECTS_TAB_FILTER_VALUES,
	LOADER_ROWS_AMOUNT,
} from './constants';
import {
	ProjectAccessGridWrapper,
	ProjectAccessTabHeader,
	ProjectAccessTabWrapper,
} from './styles';
import { ProjectAccessTabProps } from './types';

export function CommonProjectAccessTab({
	user,
	isUserTab,
	applyDefaultSorting,
}: ProjectAccessTabProps) {
	const [projectAssignmentRows, setProjectAssignmentRows] = useState<
		Array<ProjectRow>
	>(getProjectTableLoaderRows(LOADER_ROWS_AMOUNT));

	const { filters, setFilters, areFiltersApplied, resetFilters } =
		useProjectsFilters(
			isUserTab || applyDefaultSorting
				? DEFAULT_USER_PROJECTS_TAB_FILTER_VALUES
				: undefined
		);

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
		initialOrder: SortDirection.DESC,
		initialOrderBy: PROJECT_TABLE_COLUMNS.startDate.key,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserProjectAssignments(
		user.id,
		page,
		rowsPerPage,
		filters,
		order,
		orderBy
	);

	const isDataLoading = isLoading || isFetching;

	const {
		id,
		isOpen,
		handleClose,
		handleProjectEditClick,
		handleFormSubmit,
		showLoader,
		handleAddClick,
	} = useUserProjectAssignmentFormState([
		TeamboardQueriesKeys.projects,
		`${page}`,
		`${rowsPerPage}`,
		`${user.id}`,
		JSON.stringify(filters),
		order,
		orderBy,
	]);

	useEffect(() => {
		if (isDataLoading) {
			setProjectAssignmentRows(getProjectTableLoaderRows(LOADER_ROWS_AMOUNT));

			return;
		}

		if (data) {
			setProjectAssignmentRows(
				convertUserProjectLookupToRows(data.items, handleProjectEditClick)
			);
		}
	}, [data, isDataLoading]);

	const projectTablePagination = useMemo(
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

	return (
		<ProjectAccessTabWrapper>
			<ProjectAccessTabHeader>
				<TabHeader onAddClick={handleAddClick} />
			</ProjectAccessTabHeader>
			<ProjectAccessGridWrapper>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<Loader isLoading={isDataLoading} size="small">
						<ProjectsTable
							mode={TeamboardTableMode.Tab}
							pagination={projectTablePagination}
							rows={projectAssignmentRows}
							displayFilters
							filterSetters={setFilters}
							onFiltersReset={resetFilters}
							isColumnsSortable
							sortColumn={sortColumns}
							{...{
								isLoading,
								handleSortColumnChange,
								isUserTab,
								areFiltersApplied,
								filters,
							}}
						/>
					</Loader>
				</DataGridHoverRowWrapper>
			</ProjectAccessGridWrapper>
			{isOpen && (
				<TeamboardUserProjectsForm
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					projectAssignId={id}
					showLoader={showLoader}
					user={user}
				/>
			)}
		</ProjectAccessTabWrapper>
	);
}
