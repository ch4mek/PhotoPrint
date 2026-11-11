import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_HEADER_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useCallback, useMemo } from 'react';

import { useCalendarFilters } from '../../context/CalendarFiltersContext';

import { useDataGridOffsetWidth } from '@/components/Grid/hooks/useGridWidth';
import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { isChiefRole } from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { SortDirection } from '@/constants/sort';
import { useAuth, useConfiguration, useToast } from '@/context/index';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { getFullName } from '@/helpers/utils';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import dayjs from '@/lib/dayjs';
import { useTableState } from '@/pages/hooks/useTableState';
import { DEFAULT_REQUESTS_FILTERS } from '@/pages/Requests/components/ListTab/constants';
import { useRequestsFilters } from '@/pages/Requests/components/ListTab/hooks/useRequestsFilters';
import { getCurrentMonth } from '@/pages/TimeSheet/components/TimesheetPageControls/utils';
import { useGetInquiriesCalendar } from '@/queries/inquiries/useGetInquiriesCalendar';
import { useRequestUsersLookup } from '@/queries/inquiries/useRequestUsersLookup';
import { InquiryCalendarGroupRow } from '@/types/absences';
import { UserClaims } from '@/types/claims';

import {
	CALENDAR_GRID_ROW_HEIGHT,
	getAbsenceCalendarColumns,
	INITIAL_ORDER_BY_VALUE,
} from './constants';
import { useAbsenceCalendarColumns } from './hooks/useAbsenceCalendarColumns';
import {
	AbsenceCalendarGridWrapper,
	overrideCalendarGridStyles,
} from './styles';
import { getRowKey, prepareRow } from './utils';

export function AbsenceCalendarGrid() {
	const toast = useToast();
	const { role, id: authId, claims } = useAuth();
	const hasHRModuleClaim = checkIsHasClaims(
		[UserClaims.ManageHRModule],
		claims
	);

	const { date, inquiryId } = useCalendarFilters();
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const { filters, setFilters } = useRequestsFilters(DEFAULT_REQUESTS_FILTERS);

	const isDesktopWidth = useHasScreenWidth(breakpoints.desktop);
	const { gridOffsetWidth, gridRef } = useDataGridOffsetWidth();

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleUserCrossLinkClick = useCallback(
		(row: InquiryCalendarGroupRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: InquiryCalendarGroupRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const currentMonth = useMemo(
		() => getCurrentMonth(new Date(date.year, date.month)),
		[date]
	);
	const period = useMemo(
		() => currentMonth.map((periodDate) => dayjs(periodDate).toDate()),
		[currentMonth]
	);

	const {
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: DEFAULT_ROWS_PER_PAGE_INT,
		initialOrder: SortDirection.ASC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
		resetPageDeps: [filters, period, inquiryId],
	});

	const absenceColumns = useMemo(
		() => getAbsenceCalendarColumns(isDesktopWidth),
		[isDesktopWidth]
	);

	const orderBy = useMemo(
		() =>
			sortColumns
				.map((col) => {
					const { columnKey, direction } = col;

					if (columnKey === absenceColumns.department.key) {
						return `departmentId ${direction}`;
					}

					if (columnKey === absenceColumns.userFullName.key) {
						return `userFullNameRu eq null ${SortDirection.ASC}, userFullNameRu ${direction}`;
					}

					return `${columnKey} ${direction}`;
				})
				.join(','),
		[sortColumns, absenceColumns]
	);

	const { data, isFetching } = useGetInquiriesCalendar(
		page,
		Number(rowsPerPage),
		period,
		filters,
		orderBy,
		departmentTypes.length,
		inquiryId || undefined
	);

	const { data: users } = useRequestUsersLookup({
		onError: createOnErrorHandler(toast),
	});

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const options = useMemo(
		() => ({
			users: users
				? users.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
			departments: convertToSelectionModalOptions(departments, 'fullName'),
		}),
		[users, departments]
	);

	const { columns, dateColumnWidth } = useAbsenceCalendarColumns({
		period,
		gridOffsetWidth,
		absenceColumns,

		filters,
		setFilters,
		options,
		onUserClick: handleUserCrossLinkClick,
		enableCellClick,
	});

	const rows = useMemo(
		() =>
			(data?.items || [])
				.map((row) => {
					const preparedRow = prepareRow(row, period, departmentTypes);
					const canManageByRole = isChiefRole(role)
						? !isChiefRole(row.userPosition)
						: row.userId !== authId && row.isManagedByRoleRequest;
					const canManageRequest = hasHRModuleClaim || canManageByRole;

					return {
						...preparedRow,
						isManagedByRoleRequest: canManageRequest,
					};
				})
				.filter((row) => row.inquiries.length > 0),
		[data, period, departmentTypes, authId, hasHRModuleClaim, role]
	);

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		data?.total ?? 0
	);

	const overrideDataGridStyles = useMemo(
		() =>
			overrideCalendarGridStyles(
				period.length,
				absenceColumns,
				dateColumnWidth
			),
		[period, absenceColumns, dateColumnWidth]
	);

	const handleRowHeight = useCallback(
		(row: InquiryCalendarGroupRow) => CALENDAR_GRID_ROW_HEIGHT * row.maxLevel,
		[]
	);

	return (
		<Loader isLoading={isFetching}>
			<AbsenceCalendarGridWrapper>
				<DataGrid
					{...{
						rows,
						columns,
						overrideDataGridStyles,
						paginationProps,
						sortColumns,
					}}
					ref={gridRef}
					rowKeyGetter={getRowKey}
					enableVirtualization={false}
					headerRowHeight={DEFAULT_GRID_HEADER_ROW_HEIGHT}
					rowHeight={handleRowHeight}
					renderers={noRowsGridFallback}
					onSortColumnsChange={handleSortColumnChange}
				/>
			</AbsenceCalendarGridWrapper>
		</Loader>
	);
}
