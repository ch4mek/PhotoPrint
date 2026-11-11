import { useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { Loader } from '@/components/Loader';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetUserSpecializationsSnapshots } from '@/queries/user/useGetUserSpecializationsSnapshots';

import { SpecializationTable } from './components/SpecializationTable';
import { useTabSpecializationTableColumns } from './components/SpecializationTable/hooks/useTabSpecializationTableColumns';
import { useTabSpecializationTableRows } from './components/SpecializationTable/hooks/useTabSpecializationTableRows';
import { useSpecializationDetails } from './hooks/useSpecializationDetails';
import { useSpecializationFilters } from './hooks/useSpecializationFilters';
import { TabTableWrapper } from './styles';

export function HardSkillsTab() {
	const { filters, setFilters } = useSpecializationFilters();
	const { userId } = useContext(PersonalDataContext);

	const { rowsPerPage, page, setPage, handleOnPerPageChange } = useTableState({
		resetPageDeps: [filters],
	});

	const { data, isFetching, isLoading } = useGetUserSpecializationsSnapshots(
		userId,
		page,
		rowsPerPage,
		filters.specializationName || undefined
	);

	const { allDates } = useSpecializationDetails(data?.items);

	const rows = useTabSpecializationTableRows(data?.items);

	const columns = useTabSpecializationTableColumns(
		data?.items,
		allDates,
		filters,
		setFilters
	);

	const gridPagination = useMemo(
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

	const isDataLoading = isLoading || isFetching;

	return (
		<Loader isLoading={isDataLoading} size="small">
			<TabTableWrapper>
				<SpecializationTable
					allDates={allDates}
					rows={rows}
					columns={columns}
					gridPagination={gridPagination}
				/>
			</TabTableWrapper>
		</Loader>
	);
}
