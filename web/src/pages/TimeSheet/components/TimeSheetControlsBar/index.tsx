import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	defaultInitialTimesheetMode,
	timesheetModesIds,
} from '../../constants';
import { RoleDropdownWithDepartments } from '../RoleDropdownWithDepartments';
import { TimeSheetFilters } from '../TimeSheetFilters';
import {
	addWeek,
	createFilterSetter,
	getCurrentMonday,
	modifyMonth,
	selectValueByPeriod,
	subtractWeek,
} from '../TimesheetPageControls/utils';
import { UserDropdown } from '../UserDropdown';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { FilterDropDownWithArrows } from '@/components/FilterDropDownWIthArrows';
import { ProtectedView } from '@/components/ProtectedView';
import { ResetFilterButton } from '@/components/ResetFilterButton';
import { isPMO } from '@/constants/departments';
import {
	filterByPeriod,
	initialGroupFilterValue,
	NO_VALUE_FILTER_OPTION,
} from '@/constants/filters';
import {
	chiefRoles,
	groupManagerAndHead,
	isHeadOfDepartment,
	isManagerRole,
	managerRoles,
	managerRolesWithGroupManager,
	Roles,
} from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { getDateLocale } from '@/i18n/locale';
import { useGroupsLookup } from '@/queries/groups/useGroupsLookup';

import {
	AutoselectWrapper,
	FilterDropDownWithArrowsWrapper,
	FiltersBarWrapper,
	ResetButtonWrapper,
} from './styles';
import { TimeSheetControlsBarProps } from './types';

const { personalModeId } = timesheetModesIds;
const { SE } = Roles;
const { monthFilter, weekFilter } = filterByPeriod;

export function TimeSheetControlsBar({
	setDate,
	weekDisplay,
	clientFilter,
	setClient,
	projectFilter,
	setProject,
	departmentTypeFilter,
	setDepartmentTypeFilter,
	initialClientsValue,
	initialProjectsValue,
	initialUserFilterValue,
	setPeriod,
	periodFilter,
	setUserFilter,
	userFilter,
	month,
	currentWeek,
	currentDate,
	isMobileView = false,
	period,
	groupFilter,
	setGroupFilter,
	disabled: isResetDisabled,
	onClick: handlerOnClickClearButton,
	isEmployeeOrSysAdminRole,
}: TimeSheetControlsBarProps) {
	const theme = useTheme();
	const { departmentId, role } = useAuth();
	const {
		i18n: { language },
	} = useTranslation();
	const { t } = useTranslation('timesheet', { keyPrefix: 'period' });
	const { t: tFilters } = useTranslation('timesheet', {
		keyPrefix: 'filters',
	});
	const dateLocale = getDateLocale(language);

	const handleLeftArrow = () =>
		setDate((prev: Date) =>
			periodFilter === filterByPeriod.weekFilter
				? subtractWeek(prev)
				: modifyMonth(prev, 'subtract')
		);

	const handleRightArrow = () =>
		setDate((prev: Date) =>
			periodFilter === filterByPeriod.weekFilter
				? addWeek(prev)
				: modifyMonth(prev, 'add')
		);

	const handlePeriod = useCallback(
		(value: unknown) => {
			if (value === t('thisWeek')) {
				setPeriod(filterByPeriod.weekFilter);
				setDate(() => getCurrentMonday());
			}

			if (value === t('thisMonth')) {
				setPeriod(filterByPeriod.monthFilter);
				setDate(() => new Date());
			}
		},
		[setPeriod, setDate, t]
	);

	const selectedDate = selectValueByPeriod(
		periodFilter,
		currentWeek,
		weekDisplay,
		month,
		currentDate.getFullYear(),
		dateLocale
	);

	const displaySelectedDate = useMemo(() => {
		if (selectedDate === weekFilter) {
			return t('thisWeek');
		}

		if (selectedDate === monthFilter) {
			return t('thisMonth');
		}

		return selectedDate;
	}, [selectedDate, t]);

	const timePeriodOptions = useMemo(() => [t('thisWeek'), t('thisMonth')], [t]);

	const localizedInitialGroups = useMemo(
		() => ({ ...initialGroupFilterValue, name: tFilters('allGroups') }),
		[tFilters]
	);

	const displayGroupFilter = useMemo(
		() =>
			groupFilter.id === initialGroupFilterValue.id
				? localizedInitialGroups
				: groupFilter,
		[groupFilter, localizedInitialGroups]
	);

	const handleGroupChange = useMemo(
		() => createFilterSetter(setGroupFilter, initialGroupFilterValue),
		[setGroupFilter]
	);

	const departmentIdForGroupOptions = useMemo(() => {
		if (isHeadOfDepartment(role)) {
			return departmentId;
		}

		return departmentTypeFilter?.id &&
			departmentTypeFilter?.id !== defaultInitialTimesheetMode.id
			? departmentTypeFilter.id
			: null;
	}, [role, departmentId, departmentTypeFilter]);

	const { data: groups, isFetching: isGroupsFetching } = useGroupsLookup(
		departmentIdForGroupOptions,
		{
			enabled: isManagerRole(role),
		}
	);

	const groupsOptions = useMemo(
		() => (isGroupsFetching ? [] : [...(groups || []), NO_VALUE_FILTER_OPTION]),
		[isGroupsFetching]
	);

	const stylesConfig = {
		backgroundColor: theme.palette['background-white'],
	};

	const fillAvailableWidthOnMobile = isMobileView && isEmployeeOrSysAdminRole;

	return (
		<FiltersBarWrapper
			{...{ isEmployeeOrSysAdminRole }}
			fillAvailableWidth={fillAvailableWidthOnMobile}
		>
			<ProtectedView roles={managerRolesWithGroupManager}>
				<>
					{departmentTypeFilter?.id !== personalModeId && (
						<TimeSheetFilters
							{...{
								groupFilter,
								stylesConfig,
								departmentTypeFilter,
								clientFilter,
								setClient,
								projectFilter,
								setProject,
								initialProjectsValue,
								initialClientsValue,
								initialUserFilterValue,
								setUserFilter,
								userFilter,
								departmentId,
								isMobileView,
								period,
							}}
						/>
					)}

					<ProtectedView roles={groupManagerAndHead}>
						<AutoselectWrapper>
							<UserDropdown
								{...{
									departmentTypeFilter,
									setDepartmentTypeFilter,
									isMobileView,
								}}
							/>
						</AutoselectWrapper>
					</ProtectedView>
					<ProtectedView roles={chiefRoles}>
						<AutoselectWrapper>
							<RoleDropdownWithDepartments
								{...{
									departmentTypeFilter,
									setDepartmentTypeFilter,
									isMobileView,
								}}
							/>
						</AutoselectWrapper>
					</ProtectedView>
					{departmentTypeFilter?.id !== personalModeId && (
						<ProtectedView roles={managerRoles}>
							<AutoselectWrapper>
								<FilterAutoselect
									label=""
									value={displayGroupFilter}
									valueField="name"
									options={[localizedInitialGroups, ...groupsOptions]}
									onChange={handleGroupChange}
									valueToClean={localizedInitialGroups}
									getKey={extractKeyById}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
									{...{ isMobileView, stylesConfig }}
								/>
							</AutoselectWrapper>
						</ProtectedView>
					)}
				</>
			</ProtectedView>
			{isPMO(departmentId) && role === SE && (
				<>
					{departmentTypeFilter?.id !== personalModeId && (
						<TimeSheetFilters
							{...{
								isMobileView,
								clientFilter,
								setClient,
								setProject,
								projectFilter,
								initialProjectsValue,
								initialClientsValue,
								initialUserFilterValue,
								setUserFilter,
								userFilter,
								departmentId,
								period,
								stylesConfig,
								departmentTypeFilter,
							}}
						/>
					)}
					<AutoselectWrapper>
						<UserDropdown
							{...{
								departmentTypeFilter,
								setDepartmentTypeFilter,
								isMobileView,
							}}
						/>
					</AutoselectWrapper>
				</>
			)}
			<FilterDropDownWithArrowsWrapper
				fillAvailableWidth={fillAvailableWidthOnMobile}
			>
				<FilterDropDownWithArrows
					isMobileView={isMobileView && !isEmployeeOrSysAdminRole}
					fillAvailableWidthOnMobile={fillAvailableWidthOnMobile}
					onChange={handlePeriod}
					value={displaySelectedDate}
					options={timePeriodOptions}
					onLeftArrowClick={handleLeftArrow}
					onRightArrowClick={handleRightArrow}
				/>

				<ResetButtonWrapper {...{ isEmployeeOrSysAdminRole }}>
					<ResetFilterButton
						onClick={handlerOnClickClearButton}
						disabled={isResetDisabled}
					/>
				</ResetButtonWrapper>
			</FilterDropDownWithArrowsWrapper>
		</FiltersBarWrapper>
	);
}
