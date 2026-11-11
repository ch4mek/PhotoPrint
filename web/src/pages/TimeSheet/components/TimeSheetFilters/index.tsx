import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { timesheetModesIds } from '../../constants';
import { AutoselectWrapper } from '../TimeSheetControlsBar/styles';
import { TimeSheetFiltersProps } from '../TimeSheetControlsBar/types';
import { FilterOption } from '../TimesheetPageControls/types';
import { createFilterSetter } from '../TimesheetPageControls/utils';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { isPMO } from '@/constants/departments';
import { ALL_ID } from '@/constants/filters';
import { Roles, managerRolesWithGroupManager } from '@/constants/roles';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { getResourceGroupFilterParam } from '@/helpers/getResourceGroupFilterParam';
import { useUserFilter } from '@/hooks/useUserFilter';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useGetProjectsByClient } from '@/queries/useGetProjectsByClient';

const { SE } = Roles;
const { personalModeId, departmentModeId, projectsModeId, groupModeId } =
	timesheetModesIds;

export function TimeSheetFilters({
	clientFilter,
	setClient,
	projectFilter,
	setProject,
	initialClientsValue,
	initialProjectsValue,
	initialUserFilterValue,
	setUserFilter,
	userFilter,
	departmentTypeFilter,
	departmentId,
	isMobileView,
	period,
	groupFilter,
	stylesConfig,
}: TimeSheetFiltersProps) {
	const managedByPMO = departmentTypeFilter?.id === projectsModeId;

	const acceptedRoles = useMemo(
		() =>
			isPMO(departmentId)
				? [...managerRolesWithGroupManager, SE]
				: managerRolesWithGroupManager,
		[departmentId, isPMO]
	);

	const { data: clients, isFetching: isClientsFetching } = useClientsLookup({
		onlyManagedClients: managedByPMO,
		isActive: true,
	});

	const { data: projects, isFetching: isProjectsFetching } =
		useGetProjectsByClient(
			clientFilter?.id && clientFilter?.id !== 'All' ? clientFilter?.id : null,
			undefined,
			managedByPMO
		);

	const clientsOptions = useMemo(
		() => (isClientsFetching ? [] : clients || []),
		[isClientsFetching, clients]
	);

	const projectsOptions = useMemo(
		() => (isProjectsFetching ? [] : projects || []),
		[isProjectsFetching]
	);

	const userFilterOptions = useUserFilter(
		{
			period,
			clientId: clientFilter?.id !== ALL_ID ? clientFilter?.id : null,
			projectId: projectFilter?.id !== ALL_ID ? projectFilter?.id : null,
			resourceGroupId: getResourceGroupFilterParam(groupFilter),
			departmentId:
				departmentTypeFilter?.id !== projectsModeId &&
				departmentTypeFilter?.id !== personalModeId &&
				departmentTypeFilter?.id !== departmentModeId &&
				departmentTypeFilter?.id !== groupModeId
					? departmentTypeFilter?.id
					: null,
		},
		acceptedRoles,
		true,
		managedByPMO
	);

	const { t } = useTranslation('timesheet', { keyPrefix: 'filters' });

	const localizedInitialClients = useMemo(
		() => ({ ...initialClientsValue, name: t('allClients') }),
		[t, initialClientsValue]
	);
	const localizedInitialProjects = useMemo(
		() => ({ ...initialProjectsValue, name: t('allProjects') }),
		[t, initialProjectsValue]
	);
	const localizedInitialUsers = useMemo(
		() => ({ ...initialUserFilterValue, name: t('allUsers') }),
		[t, initialUserFilterValue]
	);
	const getDisplayFilter = useCallback(
		(filter: FilterOption, initial: FilterOption, localized: FilterOption) =>
			filter.id === initial.id ? localized : filter,
		[]
	);

	const displayClientFilter = getDisplayFilter(
		clientFilter,
		initialClientsValue,
		localizedInitialClients
	);
	const displayProjectFilter = getDisplayFilter(
		projectFilter,
		initialProjectsValue,
		localizedInitialProjects
	);
	const displayUserFilter = getDisplayFilter(
		userFilter,
		initialUserFilterValue,
		localizedInitialUsers
	);

	const handleSearchChange = () => {};

	const handleClientChange = useMemo(
		() => createFilterSetter(setClient, initialClientsValue),
		[setClient, initialClientsValue]
	);
	const handleProjectChange = useMemo(
		() => createFilterSetter(setProject, initialProjectsValue),
		[setProject, initialProjectsValue]
	);
	const handleUserChange = useMemo(
		() => createFilterSetter(setUserFilter, initialUserFilterValue),
		[setUserFilter, initialUserFilterValue]
	);

	return (
		<>
			<AutoselectWrapper>
				<FilterAutoselect
					label=""
					value={displayClientFilter}
					valueField="name"
					options={[localizedInitialClients, ...clientsOptions]}
					onChange={handleClientChange}
					valueToClean={localizedInitialClients}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					preventDropdownFlip
					onSearchInputChange={handleSearchChange}
					{...{ stylesConfig, isMobileView }}
				/>
			</AutoselectWrapper>

			<AutoselectWrapper>
				<FilterAutoselect
					label=""
					value={displayProjectFilter}
					valueField="name"
					options={[localizedInitialProjects, ...projectsOptions]}
					onChange={handleProjectChange}
					valueToClean={localizedInitialProjects}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					preventDropdownFlip
					onSearchInputChange={handleSearchChange}
					{...{ stylesConfig, isMobileView }}
				/>
			</AutoselectWrapper>

			<AutoselectWrapper>
				<FilterAutoselect
					label=""
					value={displayUserFilter}
					valueField="name"
					options={[localizedInitialUsers, ...userFilterOptions]}
					onChange={handleUserChange}
					valueToClean={localizedInitialUsers}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					preventDropdownFlip
					onSearchInputChange={handleSearchChange}
					{...{ stylesConfig, isMobileView }}
				/>
			</AutoselectWrapper>
		</>
	);
}
