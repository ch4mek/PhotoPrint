import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';
import { useCallback, useMemo, FormEvent } from 'react';

import { DateSelector } from '@/components/DateSelector';
import { FilterAutoselect } from '@/components/FilterAutoselect';
import { ProtectedView } from '@/components/ProtectedView';
import { ResetFilterButton } from '@/components/ResetFilterButton';
import { controlsText } from '@/constants/controlsTexts';
import { isPMO } from '@/constants/departments';
import {
	extraFiltersBy,
	initialClientsValue,
	initialProjectsValue,
	initialUserFilterValue,
	initialDepartmentsFilterValue,
	HOURS_FILTER_OPTIONS,
	NO_VALUE_FILTER_OPTION,
	initialGroupFilterValue,
} from '@/constants/filters';
import {
	Roles,
	RoleExtraOptions,
	managerRolesWithGroupManager,
	chiefRoles,
	employees,
	managerRoles,
	isHeadOfDepartment,
	isManagerRole,
} from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { generateQuarter } from '@/helpers/date';
import { extractKeyById } from '@/helpers/extractKey';
import { getPeriod } from '@/helpers/getPeriod';
import { getResourceGroupFilterParam } from '@/helpers/getResourceGroupFilterParam';
import { useUserFilter } from '@/hooks/useUserFilter';
import { generateNewReportsSearchParams } from '@/pages/Reports/components';
import { useGroupsLookup } from '@/queries/groups/useGroupsLookup';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useGetProjectsByClient } from '@/queries/useGetProjectsByClient';

import { Form, ControlsContainer, FormFields } from './styles';
import type { ReportControlsProps } from './types';
import { getGridColumnCountByRole } from './utils/getFilterCountByRole';

const { All } = extraFiltersBy;
const { CEO, CTO, HD, SE, GM } = Roles;

export function ReportsControlsBar({
	isResetDisabled,
	handleResetFilters,
	queryParams,
	showToast,
	setQueryParams,
	setDate,
	userFilter,
	clientFilter,
	projectFilter,
	departmentFilter,
	groupFilter,
	hoursFilter,
	editDate,
	timePeriod,
	handleSetTimePeriod,
	handleSetEditDate,
	handleClientsFilterChange,
	handleProjectsFilterChange,
	handleUserFilterChange,
	handleDepartmentFilterChange,
	handleHoursFilterChange,
	handleGroupFilterChange,
	isMobileView,
	handleClosePopover,
	refetch,
	isReportsFilterChanged,
	handleFiltersStatusReset,
}: ReportControlsProps) {
	const theme = useTheme();
	const { departmentId, role } = useAuth();

	const isPMOEmployee = useMemo(
		() => isPMO(departmentId) && role === SE,
		[departmentId, role]
	);

	const departmentIdForGroupOptions = useMemo(() => {
		if (isHeadOfDepartment(role)) {
			return departmentId;
		}

		return departmentFilter.id !== All ? departmentFilter.id : null;
	}, [role, departmentId, departmentFilter]);

	const { data: clients, isFetching: isClientsFetching } = useClientsLookup({
		isActive: true,
	});

	const { data: projects, isFetching: isProjectsFetching } =
		useGetProjectsByClient(
			clientFilter?.id && clientFilter?.id !== All
				? (clientFilter.id as number)
				: null
		);

	const { data: groups, isFetching: isGroupsFetching } = useGroupsLookup(
		departmentIdForGroupOptions,
		{
			enabled: isManagerRole(role),
		}
	);

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const clientsOptions = useMemo(
		() => (isClientsFetching ? [] : clients || []),
		[isClientsFetching]
	);

	const projectsOptions = useMemo(
		() => (isProjectsFetching ? [] : projects || []),
		[isProjectsFetching]
	);

	const filteredDepartments = useMemo(
		() =>
			departmentTypes?.filter(
				({ id: optionId }) => optionId !== RoleExtraOptions.AllUsers
			),
		[departmentTypes]
	);

	const groupsOptions = useMemo(
		() => (isGroupsFetching ? [] : [...(groups || []), NO_VALUE_FILTER_OPTION]),
		[isGroupsFetching]
	);

	const { month, year, quarter } = editDate;

	const quarterArray = generateQuarter(quarter ?? '');

	const monthArray = month ? [month] : [...(quarterArray ?? [])];

	const period = getPeriod(year, monthArray);

	const countOfFiltersByRole = getGridColumnCountByRole(role);

	const userFilterOptions = useUserFilter(
		{
			...queryParams,
			period,
			clientId: clientFilter?.id !== All ? clientFilter?.id : null,
			projectId: projectFilter?.id !== All ? projectFilter?.id : null,
			resourceGroupId: getResourceGroupFilterParam(groupFilter),
		},
		[CEO, CTO, HD, GM],
		true,
		isPMOEmployee
	);

	const handleQueryParamsUpdate = () => {
		setQueryParams(() =>
			generateNewReportsSearchParams({
				clientFilter,
				departmentFilter,
				editDateFilter: editDate,
				hoursFilter,
				userFilter,
				projectFilter,
				groupFilter,
			})
		);
	};

	const handleSearchChange = () => {};

	const handleSubmit = useCallback(
		(event: FormEvent<HTMLFormElement>) => {
			event.preventDefault();

			if (!isReportsFilterChanged) {
				refetch();

				return;
			}
			setDate(editDate);
			handleQueryParamsUpdate();
			handleClosePopover?.();
			handleFiltersStatusReset();
		},
		[
			editDate,
			projectFilter,
			departmentFilter,
			clientFilter,
			userFilter,
			hoursFilter,
			groupFilter,
			isReportsFilterChanged,
			isReportsFilterChanged,
		]
	);

	const handleResetButtonClick = () => {
		handleResetFilters?.();
		handleClosePopover?.();
	};

	const stylesConfig = {
		backgroundColor: theme.palette['background-white'],
		popover: {
			position: 'fixed',
		},
	} as const;

	return (
		<Form onSubmit={handleSubmit}>
			<FormFields {...{ countOfFiltersByRole }}>
				<ProtectedView roles={managerRolesWithGroupManager}>
					<>
						<FilterAutoselect
							label=""
							value={clientFilter}
							valueField="name"
							options={[initialClientsValue, ...clientsOptions]}
							onChange={handleClientsFilterChange}
							getKey={extractKeyById}
							checkIfOptionEqualToValue={checkIfOptionsEqualById}
							onSearchInputChange={handleSearchChange}
							{...{ isMobileView, stylesConfig }}
						/>
						<FilterAutoselect
							label=""
							value={projectFilter}
							valueField="name"
							options={[initialProjectsValue, ...projectsOptions]}
							onChange={handleProjectsFilterChange}
							getKey={extractKeyById}
							checkIfOptionEqualToValue={checkIfOptionsEqualById}
							onSearchInputChange={handleSearchChange}
							{...{ isMobileView, stylesConfig }}
						/>
						<FilterAutoselect
							label=""
							value={userFilter}
							valueField="name"
							options={[initialUserFilterValue, ...userFilterOptions]}
							onChange={handleUserFilterChange}
							getKey={extractKeyById}
							checkIfOptionEqualToValue={checkIfOptionsEqualById}
							onSearchInputChange={handleSearchChange}
							{...{ isMobileView, stylesConfig }}
						/>
						<ProtectedView roles={chiefRoles}>
							<FilterAutoselect
								label=""
								value={departmentFilter}
								options={[
									initialDepartmentsFilterValue,
									...filteredDepartments,
								]}
								getKey={extractKeyById}
								valueField="fullName"
								onChange={handleDepartmentFilterChange}
								onSearchInputChange={handleSearchChange}
								{...{ isMobileView, stylesConfig }}
							/>
						</ProtectedView>
					</>
				</ProtectedView>

				<ProtectedView roles={employees}>
					<FilterAutoselect
						label=""
						value={projectFilter}
						valueField="name"
						options={[initialProjectsValue, ...projectsOptions]}
						onChange={handleProjectsFilterChange}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						onSearchInputChange={handleSearchChange}
						{...{ isMobileView, stylesConfig }}
					/>
				</ProtectedView>

				<ProtectedView roles={managerRolesWithGroupManager}>
					<FilterAutoselect
						label=""
						value={hoursFilter}
						valueField="name"
						options={HOURS_FILTER_OPTIONS}
						onChange={handleHoursFilterChange}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						onSearchInputChange={handleSearchChange}
						{...{ isMobileView, stylesConfig }}
					/>
				</ProtectedView>

				<ProtectedView roles={managerRoles}>
					<FilterAutoselect
						label=""
						value={groupFilter}
						valueField="name"
						options={[initialGroupFilterValue, ...groupsOptions]}
						onChange={handleGroupFilterChange}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						onSearchInputChange={handleSearchChange}
						{...{ isMobileView, stylesConfig }}
					/>
				</ProtectedView>

				<DateSelector
					timePeriod={timePeriod}
					setTimePeriod={handleSetTimePeriod}
					date={editDate}
					setDate={handleSetEditDate}
					showToast={showToast}
					isMobileView={isMobileView}
				/>
			</FormFields>

			<ControlsContainer>
				<ResetFilterButton
					disabled={isResetDisabled}
					withIcon={!isMobileView}
					fullWidth
					onClick={handleResetFilters ? handleResetButtonClick : undefined}
				/>

				<Button type="submit" isFullWidth={isMobileView}>
					{controlsText.applyText}
				</Button>
			</ControlsContainer>
		</Form>
	);
}
