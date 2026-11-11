import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { PageHeader } from '@/components/PageHeader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { DateFormat } from '@/constants/date';
import {
	filterByPeriod,
	initialClientsValue,
	initialGroupFilterValue,
	initialProjectsValue,
	initialUserFilterValue,
} from '@/constants/filters';
import { employees, isRegularEmployee } from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import {
	TS_CLIENT_FILTER_STORAGE_KEY,
	TS_GROUP_FILTER_STORAGE_KEY,
	TS_PROJECT_FILTER_STORAGE_KEY,
	TS_ROLE_FILTER_STORAGE_KEY,
	TS_USER_FILTER_STORAGE_KEY,
} from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { useMobileView } from '@/hooks/useMobileView';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { useStickyState } from '@/hooks/useStickyState';
import dayjs from '@/lib/dayjs';
import {
	getCurrentMonday,
	getCurrentWeek,
	getCurrentMonth,
} from '@/pages/TimeSheet/components/TimesheetPageControls/utils';
import { useProjectsLookup } from '@/queries/useProjectsLookup';

import { TimesheetPageControls } from './components';
import { TimeSheet } from './components/Timesheet';
import { FilterOption } from './components/TimesheetPageControls/types';
import { TimesheetSubmitStatus } from './components/TimesheetSubmitStatus';
import { TimeSheetBreakpoints, timesheetModesIds } from './constants';
import { PublicProjectContext } from './context/projectOptionsContext';
import { TimesheetParametersContext } from './context/timesheetParametersContext';
import { TimesheetStatusContext } from './context/timesheetStatusContext';
import { useDateFilters } from './hooks/useDateFilters';
import { useDepartmentTypeFilterDebounce } from './hooks/useRoleFilterDebounce';
import { TimesheetHeaderWrapper, TimesheetPageWrapper } from './styled';
import {
	InitialFilterValue,
	TimesheetModeFilterOption,
	TimesheetParametersContextValues,
} from './types';
import { getInitialTimesheetsModeFilters } from './utils/getInitialTimesheetsModeFilter';
import { isEmployeeOrSysAdmin } from './utils/isEmployeeOrSysAdmin';
import { useGetUserTimeSheetStatusOnMonthIntersect } from './utils/useGetUserTimeSheetStatusOnMonthIntersect';

export function TimeSheetPage() {
	const { role, id, departmentId } = useAuth();

	const { t } = useTranslation('timesheet');

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const canOpenPersonalInfoPopup = !isRegularEmployee(role);

	const initialTimesheetModeFilter = useMemo(
		() => getInitialTimesheetsModeFilters(role, departmentId),
		[role, departmentId]
	);

	const isMobileView = useMobileView(TimeSheetBreakpoints.DESKTOP_MD);
	const isTabletView = useMobileView(TimeSheetBreakpoints.TABLET);

	const [clientFilter, setClientFilter] = useStickyState<
		FilterOption | InitialFilterValue
	>(initialClientsValue, TS_CLIENT_FILTER_STORAGE_KEY);

	const [projectFilter, setProjectFilter] = useStickyState<
		FilterOption | InitialFilterValue
	>(initialProjectsValue, TS_PROJECT_FILTER_STORAGE_KEY);

	const [groupFilter, setGroupFilter] = useStickyState<
		FilterOption | InitialFilterValue
	>(initialGroupFilterValue, TS_GROUP_FILTER_STORAGE_KEY);

	const [departmentTypeFilter, setDepartmentTypeFilter] =
		useStickyState<TimesheetModeFilterOption>(
			initialTimesheetModeFilter,
			TS_ROLE_FILTER_STORAGE_KEY
		);

	const isPersonalModeOn = useMemo(
		() => departmentTypeFilter.id === timesheetModesIds.personalModeId,
		[departmentTypeFilter]
	);

	const { date, setDate, periodFilter, setPeriodFilter, resetDateFilters } =
		useDateFilters(isPersonalModeOn);

	const [userFilter, setUserFilter] = useStickyState<
		FilterOption | InitialFilterValue
	>(initialUserFilterValue, TS_USER_FILTER_STORAGE_KEY);

	const debouncedDepartmentTypeFilter =
		useDepartmentTypeFilterDebounce(departmentTypeFilter);

	const isResetDisabled = useMemo(
		() =>
			clientFilter.id === initialClientsValue.id &&
			projectFilter.id === initialProjectsValue.id &&
			departmentTypeFilter?.id === initialTimesheetModeFilter.id &&
			periodFilter === filterByPeriod.weekFilter &&
			date.toLocaleDateString() === getCurrentMonday().toLocaleDateString() &&
			userFilter.id === initialUserFilterValue.id &&
			groupFilter.id === initialGroupFilterValue.id,
		[
			clientFilter,
			projectFilter,
			departmentTypeFilter,
			periodFilter,
			userFilter,
			date,
			initialTimesheetModeFilter,
			groupFilter,
		]
	);

	const isWeekFilter = periodFilter === filterByPeriod.weekFilter;

	const clearAllFilters = () => {
		setClientFilter(initialClientsValue);
		setProjectFilter(initialProjectsValue);
		setDepartmentTypeFilter(initialTimesheetModeFilter);
		setUserFilter(initialUserFilterValue);
		setGroupFilter(initialGroupFilterValue);
		resetDateFilters();
	};

	const currentWeek = useMemo(() => getCurrentWeek(date), [date]);
	const currentMonth = useMemo(() => getCurrentMonth(date), [date]);

	const weekDisplay = useMemo(() => {
		const isCurrentWeekSelected =
			getCurrentMonday()?.toLocaleDateString() === date?.toLocaleDateString();

		if (isCurrentWeekSelected) {
			return filterByPeriod.weekFilter;
		}

		const weekStart = currentWeek[0];
		const weekEnd = currentWeek[currentWeek.length - 1];

		return `${dayjs(weekStart).format(DateFormat.DD_MM_YYYY)}-${dayjs(
			weekEnd
		).format(DateFormat.DD_MM_YYYY)}`;
	}, [currentWeek, date]);

	const isProjectsModeOn = useMemo(
		() =>
			departmentTypeFilter.id === timesheetModesIds.projectsModeId
				? true
				: undefined,
		[departmentTypeFilter]
	);

	const {
		data: publicProjectOptions,
		isLoading: isProjectsLoading,
		isFetching: isProjectsFetching,
	} = useProjectsLookup({
		isPublic: true,
		isActive: true,
		onlyManagedProjects: isProjectsModeOn,
	});

	const week = dayjs(date).week();
	const month = dayjs(currentMonth[0]).month() + 1;
	const year = dayjs(currentMonth[0]).year();
	const isLoading = isProjectsLoading || isProjectsFetching;

	const period: Date[] = useMemo(
		() =>
			isWeekFilter
				? currentWeek
				: currentMonth.map((periodDate) => dayjs(periodDate).toDate()),
		[isWeekFilter, currentMonth, currentWeek]
	);

	const timesheetParameters: TimesheetParametersContextValues = useMemo(
		() => ({
			period,
			clientFilter:
				!isPersonalModeOn && clientFilter?.id !== initialClientsValue.id
					? clientFilter
					: undefined,
			projectFilter:
				!isPersonalModeOn && projectFilter.id !== initialProjectsValue.id
					? projectFilter
					: undefined,
			isProjectsModeOn,
			isPersonalModeOn,
		}),
		[
			period,
			clientFilter,
			projectFilter,
			departmentTypeFilter,
			isProjectsModeOn,
			isPersonalModeOn,
		]
	);

	const shouldGetTimesheetStatus = useMemo(
		() => employees.includes(role) && !isProjectsModeOn,
		[role, isProjectsModeOn]
	);

	const timeSheetStatus = useGetUserTimeSheetStatusOnMonthIntersect(
		currentWeek,
		month,
		year,
		id,
		isWeekFilter,
		{
			enabled: shouldGetTimesheetStatus,
		}
	);

	const shouldRenderSubmitStatusForEmployee = useMemo(
		() =>
			isRegularEmployee(role) &&
			filterByPeriod.monthFilter === periodFilter &&
			timesheetModesIds.projectsModeId !== departmentTypeFilter?.id,
		[
			role,
			filterByPeriod.monthFilter,
			periodFilter,
			timesheetModesIds.projectsModeId,
			departmentTypeFilter?.id,
		]
	);

	const isEmployeeOrSysAdminRole = useMemo(
		() => isEmployeeOrSysAdmin(role, departmentId),
		[role, departmentId]
	);

	const handleUserClick = useCallback(
		(userId: number) => {
			handlePageOpen(userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(userId: number) => canOpenPersonalInfoPopup && canOpenProfile(userId),
		[canOpenPersonalInfoPopup, canOpenProfile]
	);

	const TimesheetStatusContextValue = useMemo(
		() => ({
			role,
			departmentId,
			periodFilter,
			departmentTypeFilter,
			month,
			year,
			handleUserClick,
			enableCellClick,
		}),
		[
			role,
			departmentId,
			periodFilter,
			departmentTypeFilter,
			month,
			year,
			handleUserClick,
			enableCellClick,
		]
	);

	return (
		<TimesheetPageWrapper>
			<TimesheetStatusContext.Provider value={TimesheetStatusContextValue}>
				<PublicProjectContext.Provider value={publicProjectOptions}>
					<TimesheetParametersContext.Provider value={timesheetParameters}>
						<TimesheetHeaderWrapper
							shouldGetTimesheetStatus={shouldGetTimesheetStatus}
							isEmployeeOrSysAdminRole={isEmployeeOrSysAdminRole}
							isMonthFilter={!isWeekFilter}
						>
							{(!isMobileView || isEmployeeOrSysAdminRole) && (
								<PageHeader title={t('title')}>
									{shouldRenderSubmitStatusForEmployee && isTabletView && (
										<TimesheetSubmitStatus status={timeSheetStatus ?? null} />
									)}
								</PageHeader>
							)}
							<TimesheetPageControls
								{...{
									isMobileView,
									isEmployeeOrSysAdminRole,
									weekDisplay,
									setDate,
									setDepartmentTypeFilter,
									clientFilter,
									projectFilter,
									initialProjectsValue,
									initialClientsValue,
									initialUserFilterValue,
									periodFilter,
									setUserFilter,
									groupFilter,
									setGroupFilter,
									userFilter,
									month,
									isResetDisabled,
									shouldRenderSubmitStatusForEmployee,
									period,
								}}
								clearAllFilter={clearAllFilters}
								currentDate={date}
								departmentTypeFilter={debouncedDepartmentTypeFilter}
								setClient={setClientFilter}
								setProject={setProjectFilter}
								setPeriod={setPeriodFilter}
								currentWeek={week}
								engineerStatus={timeSheetStatus}
							/>
						</TimesheetHeaderWrapper>
						<TimeSheet
							{...{
								groupFilter,
								isLoading,
								clientFilter,
								projectFilter,
								periodFilter,
								userFilter,
								month,
								period,
								departmentId,
							}}
							userId={id}
							departmentTypeFilter={debouncedDepartmentTypeFilter}
							engineerStatus={timeSheetStatus}
						/>
					</TimesheetParametersContext.Provider>
				</PublicProjectContext.Provider>
			</TimesheetStatusContext.Provider>
		</TimesheetPageWrapper>
	);
}
