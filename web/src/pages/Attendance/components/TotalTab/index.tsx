import { useTheme } from '@emotion/react';
import { useEffect, useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { DateFormat } from '@/constants/date';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { formatDate } from '@/helpers/formatDate';
import { getFullName } from '@/helpers/utils';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetAttendancesTotal } from '@/queries/attendance/useGetAttendancesTotal';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useAllUsersLookup } from '@/queries/user/useAllUsersLookup';
import { useAttendanceTotalStore } from '@/stores/attendance/total/store';
import { createSelector } from '@/stores/helpers';

import { TotalGrid } from './components/TotalGrid';
import {
	INITIAL_ORDER_BY_VALUE,
	ATTENDANCE_TOTAL_GRID_COLUMNS,
	DEFAULT_ATTENDANCE_TOTAL_FILTERS,
	getAttendanceTotalColumns,
} from './constants';
import { useGridOptions } from './hooks/useGridOptions';
import { AttendanceTotalGridRow } from './types';
import { converMinutesToTime } from './utils/timeConverter';

export function TotalTab() {
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

	const filters = useAttendanceTotalStore(
		createSelector('userId', 'user', 'departments', 'group', 'date', 'time')
	);

	const {
		rowsPerPage,
		sortColumns,
		page,
		setPage,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
		resetPageDeps: [filters],
	});

	const { initialize, initialFilters } = useAttendanceTotalStore(
		createSelector('initialize', 'initialFilters')
	);

	const storeSetters = useAttendanceTotalStore(
		createSelector(
			'setUserId',
			'setUser',
			'setDepartments',
			'setGroup',
			'setDate',
			'setTime',
			'setAll',
			'reset'
		)
	);

	const setFilters = useMemo(
		() => ({
			userId: storeSetters.setUserId,
			user: storeSetters.setUser,
			departments: storeSetters.setDepartments,
			group: storeSetters.setGroup,
			date: storeSetters.setDate,
			time: storeSetters.setTime,
			set: storeSetters.setAll,
			reset: storeSetters.reset,
		}),
		[storeSetters]
	);

	const computedInitialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_ATTENDANCE_TOTAL_FILTERS,
		role,
		departmentId
	);

	useEffect(() => {
		initialize(computedInitialFilters);
	}, [computedInitialFilters, initialize]);

	const columnsWidths = useMemo(() => getAttendanceTotalColumns(role), [role]);

	const options = useGridOptions({ users, departmentsInfo });

	const orderBy = sortColumns
		.map((col) => {
			let { columnKey } = col;
			const { direction } = col;

			if (col.columnKey === ATTENDANCE_TOTAL_GRID_COLUMNS.department.key) {
				columnKey = 'userDepartmentId';
			} else if (col.columnKey === ATTENDANCE_TOTAL_GRID_COLUMNS.group.key) {
				columnKey = 'userResourceGroupName';
			} else if (col.columnKey === ATTENDANCE_TOTAL_GRID_COLUMNS.user.key) {
				return `userFullNameRu eq null ${SortDirection.ASC}, userFullNameRu ${direction}`;
			} else if (col.columnKey === ATTENDANCE_TOTAL_GRID_COLUMNS.time.key) {
				columnKey = 'totalTimeInMinutes';
			}

			return `${columnKey} ${col.direction}`;
		})
		.join(',');

	const { data: attendancesTotalData, isFetching: isLoadingAttendances } =
		useGetAttendancesTotal(
			Number(rowsPerPage),
			page,
			orderBy,
			filters,
			allDepartmentsLength
		);

	const rows = useMemo<AttendanceTotalGridRow[]>(() => {
		if (!attendancesTotalData?.items) {
			return [];
		}

		return attendancesTotalData.items.map((item) => {
			const { userFirstName, userFirstNameRu, userLastName, userLastNameRu } =
				item;

			const departmentFullName =
				departmentTypes?.find(
					(department) => department.id === item.userDepartmentId
				)?.fullName || '';

			const groupName =
				departmentsInfo
					?.find(({ departmentName }) => departmentName === departmentFullName)
					?.groups?.find(
						({ id: groupId }) => groupId === item.userResourceGroupId
					)?.name || '';

			return {
				id: `${item.userId}-${item.date}`,
				userId: item.userId,
				user: getFullName(
					userFirstName,
					userLastName,
					userFirstNameRu,
					userLastNameRu
				),
				department: departmentFullName,
				group: groupName,
				date: formatDate(item.date, DateFormat.DD_MM_YYYY),
				time: converMinutesToTime(item.totalTimeInMinutes),
				isUserActive: item.isUserActive,
				totalTime: '',
			};
		});
	}, [attendancesTotalData, departmentTypes, departmentsInfo]);

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		attendancesTotalData?.total || 0
	);

	return (
		<PageWrapper style={{ paddingTop: theme.spaces.m }}>
			<Loader
				isLoading={
					isLoadingDepartments || isLoadingUsers || isLoadingAttendances
				}
			>
				<TotalGrid
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
