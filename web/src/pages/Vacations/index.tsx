import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo, useState } from 'react';

import { PageWrapper } from '../styles';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PageTitles } from '@/constants/pageTitles';

import { useCalendarColumns } from './hooks/useCalendarColumns';
import { INITIAL_GRID_VALUES } from './mockData';
import {
	DateFilterWrapper,
	overrideVacationsGridStyles,
	VacationsHeaderWrapper,
} from './styles';
import { aggregateEmployeeLeaveDays, getMinDate, groupByMonth } from './utils';

export function VacationsPage() {
	const monthsMap = useMemo(() => groupByMonth(INITIAL_GRID_VALUES), []);
	const months = useMemo(() => Object.keys(monthsMap), [monthsMap]);
	const [currentPage, setCurrentPage] = useState(1);
	const [datePickerValue, setDatePickerValue] = useState(
		months[currentPage - 1]
	);
	const minDate = useMemo(() => getMinDate(INITIAL_GRID_VALUES), []);
	const initialYear = minDate.getFullYear();
	const initialMonth = minDate.getMonth() + 1;

	const columns = useMemo(
		() => useCalendarColumns(initialYear, initialMonth + (currentPage - 1)),
		[currentPage, initialYear]
	);

	const overrideVacationsGridStyle = useMemo(
		() => overrideVacationsGridStyles,
		[]
	);

	const getEmployeesForCurrentMonth = () => {
		const currentMonth = months[currentPage - 1];
		const employees = monthsMap[currentMonth] || [];

		return aggregateEmployeeLeaveDays(employees);
	};

	const handleDatePickerChange = useCallback(
		(selectedMonth: string | null) => {
			const newPage = months.indexOf(selectedMonth!) + 1;

			if (newPage > 0) {
				setCurrentPage(newPage);
				setDatePickerValue(selectedMonth!);
			}
		},
		[months]
	);

	return (
		<PageWrapper>
			<VacationsHeaderWrapper>
				<PageHeader title={PageTitles.Vacations} />
				<DateFilterWrapper>
					<FilterAutoselect
						options={months}
						value={datePickerValue}
						onChange={handleDatePickerChange}
						label="Select month"
						shouldBeClosedOnOutsideScroll
					/>
				</DateFilterWrapper>
			</VacationsHeaderWrapper>
			<DataGrid
				key={currentPage}
				columns={columns}
				rows={getEmployeesForCurrentMonth()}
				headerRowHeight={28}
				rowHeight={40}
				overrideDataGridStyles={overrideVacationsGridStyle}
				scrollToTopOnRowsChange={false}
				renderers={noRowsGridFallback}
			/>
		</PageWrapper>
	);
}
