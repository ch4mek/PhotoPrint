import { useMemo } from 'react';

import { ProjectExitsGridOptions } from '../types';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { getFullName } from '@/helpers/utils';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useGetClosingReasons } from '@/queries/projectExits/useGetClosingReasons';
import { useProjectExitUsersLookup } from '@/queries/projectExits/useProjectExitUsersLookup';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useProjectsLookup } from '@/queries/useProjectsLookup';

export const useFilterOptions = () => {
	const { role, departmentId } = useAuth();
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const { data: usersLookup } = useProjectExitUsersLookup();
	const userOptions = useMemo(
		() =>
			(usersLookup || []).map(
				({ firstName, lastName, firstNameRu, lastNameRu }) =>
					getFullName(firstName, lastName, firstNameRu, lastNameRu)
			),
		[usersLookup]
	);

	const { data: clientsLookup } = useClientsLookup();
	const clientOptions = useMemo(
		() => clientsLookup?.map(({ name }) => name) || [],
		[clientsLookup]
	);

	const { data: projectsLookup } = useProjectsLookup();
	const projectOptions = useMemo(
		() => projectsLookup?.map(({ name }) => name) || [],
		[projectsLookup]
	);

	const currentDepartment = useMemo(
		() => departmentTypes.find(({ id }) => id === departmentId)?.fullName,
		[departmentTypes, departmentId]
	);

	const { data: departmentsInfo } = useGetDepartments();
	const groupOptions = useMemo(
		() =>
			getGroupNamesFromDepartments(
				departmentsInfo ?? [],
				role,
				currentDepartment
			),
		[departmentsInfo, role, currentDepartment]
	);

	const { data: closingReasons } = useGetClosingReasons();
	const closingReasonOptions = useMemo(
		() => closingReasons?.map(({ name }) => name) || [],
		[closingReasons]
	);

	const options = useMemo(
		(): ProjectExitsGridOptions => ({
			users: userOptions,
			departments: convertToSelectionModalOptions(departments, 'fullName'),
			groups: groupOptions,
			clients: clientOptions,
			projects: projectOptions,
			reasons: closingReasonOptions,
		}),
		[
			userOptions,
			departments,
			groupOptions,
			clientOptions,
			projectOptions,
			closingReasonOptions,
		]
	);

	return { options, allDepartmentsLength: departments.length };
};
