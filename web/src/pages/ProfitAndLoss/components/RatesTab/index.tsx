import { useCallback, useMemo, useRef } from 'react';

import { useGridRowsExpansion } from '../../hooks/useGridRowsExpansion';
import { useProfitAndLossFilterOptions } from '../../hooks/useProfitAndLossFilterOptions';
import { useProfitAndLossFilters } from '../../hooks/useProfitAndLossFilters';
import { HeaderInnerBlockWrapper, PageHeaderWrapper } from '../../styles';
import { DateValueFilterControls } from '../DateValueFilterControls';
import { OpenRowsToggle } from '../OpenRowsToggle';

import { useDateValueFilter } from '@/components/DateValueFilter/hooks/useDateValueFilter';
import { Loader } from '@/components/Loader';
import { SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { createGridPagination } from '@/helpers/createGridPagination';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetRates } from '@/queries/rates/useGetRates';

import { RateHistoryPopup } from './components/RateHistoryPopup';
import { RatesGrid } from './components/RatesGrid';
import { ratesGridColumns } from './constants';
import { useRatesGridRows } from './hooks/useRatesGridRows';
import { RateHistoryContext, RatesGridRow } from './types';
import { getExpandableParentIds } from './utils/getExpandableParentIds';
import { getRateHistoryContextFromRows } from './utils/getRateHistoryContextFromRows';

export function RatesTab() {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const canOpenProfile = usePersonalInfoPopupGuard();

	const {
		id: historyContext,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<RateHistoryContext>();

	const { date, setDate, isResetDisabled, resetDate } = useDateValueFilter();
	const { filters, setFilters, isResetFiltersDisabled } =
		useProfitAndLossFilters();

	const year = date.year();
	const month = date.month() + 1;

	const {
		sortColumns,
		rowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialOrder: SortDirection.ASC,
		initialOrderBy: ratesGridColumns.id.key,
		resetPageDeps: [filters, year, month],
	});

	const { options: filterOptions, allDepartmentsLength } =
		useProfitAndLossFilterOptions(year, month);

	const { data: ratesData, isFetching } = useGetRates(
		Number(rowsPerPage),
		page,
		order,
		orderBy,
		year,
		month,
		filters,
		allDepartmentsLength
	);

	const expandableParentIds = useMemo(
		() => getExpandableParentIds(ratesData?.items ?? []),
		[ratesData?.items]
	);

	const {
		expandedIds,
		isAllRowsOpen,
		handleOpenRowsToggleChange,
		handleToggleExpand,
	} = useGridRowsExpansion(expandableParentIds);

	const rowsRef = useRef<RatesGridRow[]>([]);

	const handleOpenRateHistory = useCallback(
		(rowId?: string | number) => () => {
			if (rowId == null) {
				return;
			}

			const context = getRateHistoryContextFromRows(
				rowId.toString(),
				rowsRef.current
			);

			if (context) {
				handleOpen(context);
			}
		},
		[handleOpen]
	);

	const rows = useRatesGridRows({
		items: ratesData?.items ?? [],
		expandedIds,
		departmentTypes,
		canOpenProfile,
		handleOpenRateHistory,
	});

	rowsRef.current = rows;

	const paginationProps = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				Number(rowsPerPage),
				handleOnPerPageChange,
				ratesData?.total ?? 0
			),
		[
			rows.length,
			page,
			setPage,
			rowsPerPage,
			handleOnPerPageChange,
			ratesData?.total,
		]
	);

	return (
		<>
			<PageWrapper>
				<PageHeaderWrapper>
					<DateValueFilterControls
						onDateChange={setDate}
						onReset={resetDate}
						{...{ date, isResetDisabled }}
					/>

					<HeaderInnerBlockWrapper>
						<OpenRowsToggle
							checked={isAllRowsOpen}
							onChange={handleOpenRowsToggleChange}
						/>
					</HeaderInnerBlockWrapper>
				</PageHeaderWrapper>

				<Loader isLoading={isFetching}>
					<RatesGrid
						{...{
							rows,
							filters,
							setFilters,
							isResetFiltersDisabled,
							sortColumns,
							handleSortColumnChange,
							paginationProps,
							filterOptions,
						}}
						onToggleExpand={handleToggleExpand}
					/>
				</Loader>
			</PageWrapper>

			{isOpen && historyContext && (
				<RateHistoryPopup
					onClose={handleClose}
					historyContext={historyContext}
				/>
			)}
		</>
	);
}
