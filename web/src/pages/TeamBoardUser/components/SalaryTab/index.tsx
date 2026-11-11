import { useCallback, useContext, useMemo, useRef } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { useSalaryFormState } from '../../hooks/useSalaryFormState';
import { SalaryTable } from '../SalaryTable';
import { SALARY_COLUMN_KEYS } from '../SalaryTable/constants';

import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { UserSalaryForm } from '@/forms/TeamboardForm/UserSalary';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserSalaries } from '@/queries/teamboard/useUserSalaries';
import { TeamboardTableMode } from '@/types/teamboard';

import { useSalaryFilters } from './hooks/useHardSkillsFilters';
import { getLoaderRows, mapSalaryResponseToRows } from './utils';

export function SalaryTab() {
	const { filters, areFiltersApplied, resetFilters, setFilters } =
		useSalaryFilters();

	const {
		rowsPerPage,
		setRowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: SALARY_COLUMN_KEYS.date,
		resetPageDeps: [filters],
	});
	const { userId } = useContext(PersonalDataContext);

	const previousRowsCountRef = useRef<number>(1);

	const { data, isLoading, isFetching } = useUserSalaries({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		orderBy: `${capitalizeString(orderBy)} ${order}`,
		filters,
		mode: TeamboardTableMode.Tab,
	});

	const {
		id,
		handleClose,
		handleAddClick,
		handleFormSubmit,
		isOpen,
		handleSalaryEditClick,
	} = useSalaryFormState({
		mode: TeamboardTableMode.Tab,
	});

	const salaryTableRows = useMemo(() => {
		if (isLoading || isFetching) {
			return getLoaderRows(previousRowsCountRef.current);
		}

		if (!data) {
			return [];
		}

		previousRowsCountRef.current = data.items.length;

		return mapSalaryResponseToRows(data.items, handleSalaryEditClick);
	}, [data, isLoading, isFetching, handleSalaryEditClick]);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const salaryTablePagination = useMemo(
		() =>
			createGridPagination(
				salaryTableRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[salaryTableRows.length, page, rowsPerPage, data?.total]
	);

	const isTableLoading = isLoading || isFetching;

	return (
		<>
			<TabHeader onAddClick={handleAddClick} />
			<Loader isLoading={isTableLoading} size="small">
				<SalaryTable
					mode={TeamboardTableMode.Tab}
					rows={salaryTableRows}
					filters={filters}
					areFiltersApplied={areFiltersApplied}
					onFiltersReset={resetFilters}
					filtersSetter={setFilters}
					pagination={salaryTablePagination}
					isLoading={isTableLoading}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
				/>
			</Loader>

			{isOpen && (
				<UserSalaryForm
					userId={userId as number}
					id={id as number}
					onSubmit={handleFormSubmit}
					handleClose={handleClose}
				/>
			)}
		</>
	);
}
