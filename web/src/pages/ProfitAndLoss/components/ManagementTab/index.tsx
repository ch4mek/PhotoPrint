import { Button } from '@mms/mms-ui-library/components/Button';
import { useMemo } from 'react';

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
import { useGetManagement } from '@/queries/rates/useGetManagement';

import { ExchangeRatePopup } from './components/ExchangeRatePopup';
import { ManagementGrid } from './components/ManagementGrid';
import { EXCHANGE_RATE_BUTTON_TEXT, managementGridColumns } from './constants';
import {
	convertManagementItemsIntoRows,
	getExpandableParentIds,
} from './utils';

export function ManagementTab() {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();
	const canOpenProfile = usePersonalInfoPopupGuard();
	const { date, setDate, isResetDisabled, resetDate } = useDateValueFilter();
	const { filters, setFilters, isResetFiltersDisabled } =
		useProfitAndLossFilters();
	const {
		isOpen: isExchangeRatePopupOpen,
		handleOpen: handleOpenExchangeRatePopup,
		handleClose: handleCloseExchangeRatePopup,
	} = useShowModal();

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
		initialOrderBy: managementGridColumns.id.key,
		resetPageDeps: [filters, year, month],
	});

	const { options: filterOptions, allDepartmentsLength } =
		useProfitAndLossFilterOptions(year, month);

	const { data: managementData, isFetching } = useGetManagement({
		top: Number(rowsPerPage),
		page,
		order,
		orderBy,
		year,
		month,
		filters,
		allDepartmentsLength,
	});

	const expandableParentIds = useMemo(
		() => getExpandableParentIds(managementData?.items ?? []),
		[managementData?.items]
	);

	const {
		expandedIds,
		isAllRowsOpen,
		handleOpenRowsToggleChange,
		handleToggleExpand,
	} = useGridRowsExpansion(expandableParentIds);

	const rows = useMemo(
		() =>
			convertManagementItemsIntoRows(
				managementData?.items ?? [],
				expandedIds,
				departmentTypes,
				canOpenProfile
			),
		[managementData?.items, expandedIds, departmentTypes, canOpenProfile]
	);

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		managementData?.total ?? 0
	);

	const summary = useMemo(
		() => ({
			expenses: managementData?.expenses ?? null,
			profit: managementData?.profit ?? null,
			incomeBynAmount: managementData?.incomeBynAmount ?? null,
			salaryAmount: managementData?.salaryAmount ?? null,
			averageKpi: managementData?.averageKpi ?? null,
			averageKpiColor: managementData?.averageKpiColor ?? null,
		}),
		[managementData]
	);

	const handleExchangeRateButtonClick = () => {
		handleOpenExchangeRatePopup();
	};

	return (
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
					<Button
						color="secondary"
						isFullWidth={false}
						onClick={handleExchangeRateButtonClick}
					>
						{EXCHANGE_RATE_BUTTON_TEXT}
					</Button>
				</HeaderInnerBlockWrapper>
			</PageHeaderWrapper>

			<Loader isLoading={isFetching}>
				<ManagementGrid
					{...{
						rows,
						filters,
						setFilters,
						isResetFiltersDisabled,
						sortColumns,
						handleSortColumnChange,
						paginationProps,
						filterOptions,
						summary,
					}}
					onToggleExpand={handleToggleExpand}
				/>
			</Loader>

			{isExchangeRatePopupOpen && (
				<ExchangeRatePopup
					date={date}
					handleClose={handleCloseExchangeRatePopup}
				/>
			)}
		</PageWrapper>
	);
}
