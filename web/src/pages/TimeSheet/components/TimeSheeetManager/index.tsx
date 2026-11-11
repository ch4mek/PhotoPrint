import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import React, {
	createContext,
	useCallback,
	useContext,
	useEffect,
	useLayoutEffect,
	useMemo,
	useState,
	UIEvent,
} from 'react';

import { useDataGridOffsetWidth } from '../../hooks/useGridWidth';
import { useTotalColumnsStuck } from '../../hooks/useTotalColumnsStuck';
import { getHeaderRowHeight } from '../../utils';

import { Loader } from '@/components/Loader';
import { ALL_ID, filterByPeriod } from '@/constants/filters';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { useAuth } from '@/context/AuthContext';
import { getResourceGroupFilterParam } from '@/helpers/getResourceGroupFilterParam';
import { isAtScrollBottom } from '@/helpers/isAtScrollBottom';
import { useMobileView } from '@/hooks/useMobileView';
import { MIN_MANAGER_ROWS_COUNT } from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { TimesheetParametersContext } from '@/pages/TimeSheet/context/timesheetParametersContext';
import { useGetTimesheetManager } from '@/queries/timesheet';

import { useTimeSheetManagerColumns } from './hooks/useTimeSheetManagerColumns';
import { useTimeSheetManagerRows } from './hooks/useTimeSheetManagerRows';
import { overrideManagerTimesheetStyles } from './styles';
import { TimeSheetManagerProps, TimeSheetManagerRow } from './types';
import { getRowHeight } from './utils/getRowHeight';
import { getRowKey } from './utils/getRowKey';

export const ManagerContext = createContext<{
	areColumnsFrozen: boolean;
	isMobileView: boolean;
}>({
	areColumnsFrozen: false,
	isMobileView: true,
});

export function TimeSheetManagerInner({
	departmentTypeFilter,
	clientFilter,
	projectFilter,
	userFilter,
	periodFilter,
	period,
	groupFilter,
}: TimeSheetManagerProps) {
	const { role } = useAuth();
	const [dataGridRows, setDataGridRows] = useState<Array<TimeSheetManagerRow>>(
		[]
	);

	const [page, setPage] = useState<number>(0);

	const isMobileView = useMobileView();

	const { desktopXS } = breakpoints;
	const isTabletWidth = useHasScreenWidth(desktopXS);

	const { gridOffsetWidth, areColumnsFrozen, gridRef } =
		useDataGridOffsetWidth();

	const isWeekFilter = periodFilter === filterByPeriod.weekFilter;

	const { isPersonalModeOn } = useContext(TimesheetParametersContext);

	const { data, isFetching, dataUpdatedAt } = useGetTimesheetManager(
		page,
		DEFAULT_ROWS_PER_PAGE_INT,
		period,
		departmentTypeFilter,
		clientFilter.id !== ALL_ID ? clientFilter.id : null,
		projectFilter.id !== ALL_ID ? projectFilter.id : null,
		userFilter.id !== ALL_ID ? userFilter.id : null,
		getResourceGroupFilterParam(groupFilter),
		isPersonalModeOn
	);

	const rowHeight = getRowHeight(
		role,
		isTabletWidth,
		periodFilter,
		departmentTypeFilter
	);

	const handleScrollToBottom = useCallback(
		(event: UIEvent<HTMLDivElement>) => {
			if (
				!data ||
				data.usersTimesheets.total <= (page + 1) * DEFAULT_ROWS_PER_PAGE_INT ||
				!isAtScrollBottom(event)
			) {
				return;
			}

			setPage(page + 1);
		},
		[data, page]
	);

	const columns = useTimeSheetManagerColumns(
		period,
		!isWeekFilter,
		gridOffsetWidth,
		areColumnsFrozen,
		isMobileView
	);

	const shouldDisplayLoader = isFetching && page === 0;

	const rows = useTimeSheetManagerRows(
		data?.usersTimesheets.items,
		dataUpdatedAt,
		role,
		period
	);

	useEffect(() => {
		setPage(0);
		setDataGridRows([]);
	}, [
		clientFilter,
		projectFilter,
		departmentTypeFilter,
		periodFilter,
		period,
		userFilter,
		groupFilter,
	]);

	useEffect(() => {
		if (
			shouldDisplayLoader &&
			dataGridRows.length &&
			gridRef.current?.element
		) {
			gridRef.current.element.scrollTop = 0;
			gridRef.current.element.scrollLeft = 0;
		}
	}, [
		shouldDisplayLoader,
		clientFilter,
		projectFilter,
		departmentTypeFilter,
		periodFilter,
		period,
		userFilter,
	]);

	useLayoutEffect(() => {
		setDataGridRows((prevDataGridRows) => {
			if (
				(prevDataGridRows.length && !prevDataGridRows[0].userId) ||
				page === 0
			) {
				return rows;
			}

			return [...prevDataGridRows, ...rows];
		});
	}, [rows, page]);

	const headerHeight = getHeaderRowHeight(true);

	const onRowsChange = useCallback((newRows: TimeSheetManagerRow[]) => {
		setDataGridRows(newRows);
	}, []);

	const overrideManagerGridStyles = useMemo(
		() =>
			overrideManagerTimesheetStyles(
				period.length,
				isMobileView,
				areColumnsFrozen,
				rowHeight,
				headerHeight
			),
		[period.length, areColumnsFrozen, isMobileView, rowHeight, headerHeight]
	);

	const handleRowHeight = (row: TimeSheetManagerRow) =>
		row?.userTimesheetRowHeight && row?.isExpanded
			? row.userTimesheetRowHeight
			: rowHeight;

	const contextValue = useMemo(
		() => ({
			areColumnsFrozen,
			isMobileView,
		}),
		[areColumnsFrozen, isMobileView]
	);

	useTotalColumnsStuck(shouldDisplayLoader);

	return (
		<ManagerContext.Provider value={contextValue}>
			<Loader isLoading={shouldDisplayLoader}>
				<DataGrid
					columns={columns}
					rows={dataGridRows}
					ref={gridRef}
					onRowsChange={onRowsChange}
					rowKeyGetter={getRowKey}
					enableVirtualization
					headerRowHeight={headerHeight}
					overrideDataGridStyles={overrideManagerGridStyles}
					rowHeight={handleRowHeight}
					scrollToTopOnRowsChange={false}
					renderers={noRowsGridFallback}
					minRowsCount={MIN_MANAGER_ROWS_COUNT}
					onScroll={handleScrollToBottom}
				/>
			</Loader>
		</ManagerContext.Provider>
	);
}

export const TimeSheetManager = React.memo(TimeSheetManagerInner);
