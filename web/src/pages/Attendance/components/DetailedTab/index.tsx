import { useTheme } from '@emotion/react';
import { useEffect, useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { DateFormat, TIME_FORMAT } from '@/constants/date';
import { INITIAL_ORDER_BY_VALUE } from '@/constants/equipment';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { FULL_HOURS_IN_DAY, TIMEZONE_OFFSET } from '@/constants/time';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { formatDate } from '@/helpers/formatDate';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import dayjs from '@/lib/dayjs';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetAttendancesDetailed } from '@/queries/attendance/useGetAttendancesDetailed';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useAllUsersLookup } from '@/queries/user/useAllUsersLookup';
import { useAttendanceDetailedStore } from '@/stores/attendance/detailed/store';
import { createSelector } from '@/stores/helpers';
import { AttendenceDirection } from '@/types/attendances';

import { DetailedGrid } from './components/DetailedGrid';
import {
	ATTENDANCE_DETAILED_GRID_COLUMNS,
	DEFAULT_ATTENDANCE_DETAILED_FILTERS,
	getAttendanceDetailedColumns,
} from './constants';
import { useGridOptions } from './hooks/useGridOptions';
import { AttendanceDetailedGridRow } from './types';

const CHECKED_AT_FIELD = 'checkedAtUtc';

export function DetailedTab() {
	const { role, departmentId } = useAuth();

	const theme = useTheme();
	const toast = useToast();

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const allDepartmentsLength = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION].length,
		[departmentTypes]
	);

	const { data: users, isLoading: isLoadingUsers } = useAllUsersLookup(toast);

	const { data: departmentsInfo, isLoading: isLoadingDepartments } =
		useGetDepartments();

	const filters = useAttendanceDetailedStore(
		createSelector(
			'id',
			'user',
			'departments',
			'group',
			'direction',
			'door',
			'location',
			'date',
			'time'
		)
	);

	const {
		rowsPerPage,
		sortColumns,
		handleSortColumnChange,
		page,
		setPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
		resetPageDeps: [filters],
	});

	const { initialize, initialFilters } = useAttendanceDetailedStore(
		createSelector('initialize', 'initialFilters')
	);

	const storeSetters = useAttendanceDetailedStore(
		createSelector(
			'setId',
			'setUser',
			'setDepartments',
			'setGroup',
			'setDirection',
			'setDoor',
			'setLocation',
			'setDate',
			'setTime',
			'setAll',
			'reset'
		)
	);

	const setFilters = useMemo(
		() => ({
			id: storeSetters.setId,
			user: storeSetters.setUser,
			departments: storeSetters.setDepartments,
			group: storeSetters.setGroup,
			direction: storeSetters.setDirection,
			door: storeSetters.setDoor,
			location: storeSetters.setLocation,
			date: storeSetters.setDate,
			time: storeSetters.setTime,
			set: storeSetters.setAll,
			reset: storeSetters.reset,
		}),
		[storeSetters]
	);

	const computedInitialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_ATTENDANCE_DETAILED_FILTERS,
		role,
		departmentId
	);

	useEffect(() => {
		initialize(computedInitialFilters);
	}, [computedInitialFilters, initialize]);

	const columnsWidths = useMemo(
		() => getAttendanceDetailedColumns(role),
		[role]
	);

	const options = useGridOptions({ users, departmentsInfo });

	const orderBy = sortColumns
		.map((col) => {
			if (col.columnKey === ATTENDANCE_DETAILED_GRID_COLUMNS.user.key) {
				return `userFullNameRu eq null ${SortDirection.ASC}, userFullNameRu ${col.direction}`;
			}

			if (col.columnKey === ATTENDANCE_DETAILED_GRID_COLUMNS.date.key) {
				return `${CHECKED_AT_FIELD} ${col.direction}`;
			}

			if (col.columnKey === ATTENDANCE_DETAILED_GRID_COLUMNS.time.key) {
				return `((hour(${CHECKED_AT_FIELD}) add ${TIMEZONE_OFFSET}) mod ${FULL_HOURS_IN_DAY}) ${col.direction}, minute(${CHECKED_AT_FIELD}) ${col.direction}`;
			}

			return `${col.columnKey} ${col.direction}`;
		})
		.join(',');

	const { data: attendancesData, isFetching: isLoadingAttendances } =
		useGetAttendancesDetailed(
			Number(rowsPerPage),
			page,
			orderBy,
			filters,
			allDepartmentsLength
		);

	const rows = useMemo<AttendanceDetailedGridRow[]>(() => {
		if (!attendancesData?.items) {
			return [];
		}

		return attendancesData.items.map((item) => {
			const checkedAtDate = dayjs
				.utc(item.checkedAtUtc)
				.add(TIMEZONE_OFFSET, 'hour');
			const time = checkedAtDate.format(TIME_FORMAT);

			return {
				id: String(item.id),
				userFullNameRu: item.userFullNameRu || '',
				userFullName: item.userFullName || '',
				user: item.userFullNameRu?.trim()
					? `${item.userFullNameRu} (${item.userFullName})`
					: item.userFullName || '',
				department: item.userDepartmentId || '',
				userDepartmentId:
					departmentTypes?.find(
						(department) => department.id === item.userDepartmentId
					)?.fullName || '',
				group: item.userResourceGroupName || '',
				userResourceGroupName: item.userResourceGroupName || '',
				direction:
					item.direction === AttendenceDirection.Enter
						? AttendenceDirection.Enter
						: AttendenceDirection.Exit,
				doorNumber: item.door,
				door: item.door,
				location: item.location,
				checkedAt: item.checkedAtUtc,
				date: formatDate(checkedAtDate, DateFormat.DD_MM_YYYY),
				time,
				isUserActive: item.isUserActive,
			};
		});
	}, [attendancesData, departmentTypes]);

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		attendancesData?.total || 0
	);

	return (
		<PageWrapper style={{ paddingTop: theme.spaces.m }}>
			<Loader
				isLoading={
					isLoadingDepartments || isLoadingUsers || isLoadingAttendances
				}
			>
				<DetailedGrid
					rows={rows}
					{...{
						columnsWidths,
						sortColumns,
						handleSortColumnChange,
						paginationProps,
						options: options.defaultOptions,
						filters,
						setFilters,
						initialFilters,
					}}
				/>
			</Loader>
		</PageWrapper>
	);
}
