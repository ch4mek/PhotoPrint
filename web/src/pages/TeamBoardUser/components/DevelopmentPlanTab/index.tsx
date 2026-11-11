import { useContext, useLayoutEffect, useMemo, useState } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { useDevelopmentPlanMutations } from '../../hooks/useDevelopmentPlanMutations';
import { DevelopmentPlanMutationMode } from '../../types';
import { convertDevelopmentPlanResponseToRows } from '../../utils';
import { DevelopmentPlanTable } from '../DevelopmentPlanTable';

import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { STATUS_FIELD_NAME } from '@/constants/developmentPlan';
import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { DevelopmentPlanForm } from '@/forms/TeamboardForm/DevelopmentPlan';
import { DevelopmentPlanApprovingForm } from '@/forms/TeamboardForm/DevelopmentPlanApproving';
import { DevelopmentPlanDeleteConfirmation } from '@/forms/TeamboardForm/DevelopmentPlanDeleteConfirmation';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserDevelopmentPlans } from '@/queries/useUserDevelopmentPlans';
import { DevelopmentPlanRow } from '@/types/developmentPlan';
import { TeamboardTableMode } from '@/types/teamboard';

import { useDevelopmentPlanFilters } from './hooks/useDevelopmentPlanFilters';

export function DevelopmentPlanTab() {
	const { userId } = useContext(PersonalDataContext);
	const [rows, setRows] = useState<Array<DevelopmentPlanRow>>([]);

	const { filters, setFilters, areFiltersApplied } =
		useDevelopmentPlanFilters();

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

	const { data, isLoading, isFetching, refetch } = useUserDevelopmentPlans({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		orderBy: getOrderValueForDevelopmentPlans(orderBy, order),
		filters,
	});

	const {
		id,
		isOpen,
		mutationMode,
		selectedDevelopmentPlanStartDate,
		handleCloseClick,
		handleAddClick,
		handleAddOrEditFormSubmit,
		handleApproveFormSubmit,
		isDeleteLoading,
		handleDeleteDevelopmentPlan,
		...developmentPlanOperations
	} = useDevelopmentPlanMutations({ refetch, mode: TeamboardTableMode.Tab });

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

	useLayoutEffect(() => {
		if (!isLoading && data?.items) {
			setRows(
				convertDevelopmentPlanResponseToRows(
					data.items,
					developmentPlanOperations
				)
			);
		}
	}, [data, isLoading]);

	const isDataLoading = isLoading || isFetching;

	return (
		<TabTableWrapper>
			<TabHeader onAddClick={handleAddClick} />
			<Loader isLoading={isDataLoading} size="small">
				<DevelopmentPlanTable
					mode={TeamboardTableMode.Tab}
					rows={rows}
					setRows={setRows}
					filter={filters}
					setFilter={setFilters}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					areFiltersApplied={areFiltersApplied}
					pagination={developmentPlanTablePagination}
					isLoading={isLoading}
				/>
			</Loader>

			{isOpen && mutationMode === DevelopmentPlanMutationMode.AddOrEdit && (
				<DevelopmentPlanForm
					id={id}
					handleClose={handleCloseClick}
					onSubmit={handleAddOrEditFormSubmit}
				/>
			)}
			{isOpen &&
				selectedDevelopmentPlanStartDate &&
				mutationMode === DevelopmentPlanMutationMode.Approve && (
					<DevelopmentPlanApprovingForm
						startDate={selectedDevelopmentPlanStartDate}
						handleClose={handleCloseClick}
						onSubmit={handleApproveFormSubmit}
					/>
				)}
			{isOpen &&
				selectedDevelopmentPlanStartDate &&
				mutationMode === DevelopmentPlanMutationMode.Delete && (
					<DevelopmentPlanDeleteConfirmation
						id={id}
						selectedDate={selectedDevelopmentPlanStartDate}
						handleClose={handleCloseClick}
						onSubmit={handleDeleteDevelopmentPlan}
						isLoading={isDeleteLoading}
					/>
				)}
		</TabTableWrapper>
	);
}
