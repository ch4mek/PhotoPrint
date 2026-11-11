import { useMemo } from 'react';

import { TeamboardOptions } from '../types';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { chiefRoles, isManagerRole, Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { findById } from '@/helpers/findById';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { getFullName } from '@/helpers/utils';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useTeamboardUsersLookup } from '@/queries/teamboard/useTeamboardUsersLookup';

export const useTeamboardOptions = () => {
	const { role, departmentId, id: currentUserId } = useAuth();
	const toast = useToast();

	const {
		persistentValues: { locationTypes },
		configurableValues: {
			departmentTypes,
			countries,
			seniorityLevels,
			englishLevels,
			roleTypes,
		},
	} = useConfiguration();

	const { data: users } = useTeamboardUsersLookup({
		onError: createOnErrorHandler(toast),
	});

	const { data: departmentsInfo } = useGetDepartments({
		enabled: isManagerRole(role),
	});

	const registeredLocationCountries = countries.map((item) => ({
		...item,
		id: String(item.id),
		description: item.name,
	}));

	const departmentOptions = useMemo(() => {
		const allDepartments = convertToSelectionModalOptions(
			departmentTypes,
			'fullName'
		);

		return [...allDepartments, NO_VALUE_FILTER_OPTION];
	}, [departmentTypes]);

	const options: TeamboardOptions = useMemo(() => {
		const department = findById(departmentTypes, departmentId);

		return {
			location: [...(locationTypes || []), NO_VALUE_FILTER_OPTION],
			registeredLocation: [...(registeredLocationCountries || [])],
			position: roleTypes?.filter(
				(position) => !chiefRoles.includes(position.id as Roles)
			),
			users: users
				? users
						.filter(({ id }) => id !== currentUserId)
						.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
							getFullName(firstName, lastName, firstNameRu, lastNameRu)
						)
				: [],
			hardSkills: convertToSelectionModalOptions(seniorityLevels, 'name'),
			englishLevels: convertToSelectionModalOptions(englishLevels, 'name'),
			departments: departmentOptions,
			groups: getGroupNamesFromDepartments(
				departmentsInfo || [],
				role,
				department?.fullName
			),
		};
	}, [
		countries,
		locationTypes,
		roleTypes,
		users,
		role,
		seniorityLevels,
		englishLevels,
		departmentOptions,
		departmentsInfo,
		departmentTypes,
		departmentId,
		currentUserId,
	]);

	return { departmentOptions, options };
};
