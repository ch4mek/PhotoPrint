import { useMemo } from 'react';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getAvailablePositions } from '@/helpers/getAvailablePosistions';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { mapNamesFromArray } from '@/helpers/mapNamesFromArray';
import { getFullName } from '@/helpers/utils';
import {
	UseGridOptionsProps,
	UsersOptions,
} from '@/pages/AdminPanel/components/UsersTab/types';

export const useGridOptions = ({
	departmentsInfo,
	users,
	hasUsersModuleAccess,
}: UseGridOptionsProps) => {
	const { role, departmentId } = useAuth();

	const {
		configurableValues: { departmentTypes, roleTypes },
		persistentValues: { locationTypes },
	} = useConfiguration();

	const availablePositions = useMemo(
		() =>
			mapNamesFromArray(
				getAvailablePositions(
					roleTypes,
					role,
					departmentId,
					hasUsersModuleAccess
				)
			),
		[role, departmentId, roleTypes, hasUsersModuleAccess]
	);

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const options: UsersOptions = useMemo(
		() => ({
			location: [...locationTypes, NO_VALUE_FILTER_OPTION],
			department: convertToSelectionModalOptions(departments, 'fullName'),
			position: roleTypes,
			group: getGroupNamesFromDepartments(
				departmentsInfo ?? [],
				role,
				departmentTypes.find(
					({ id: currentDepartmentId }) => currentDepartmentId === departmentId
				)?.fullName,
				hasUsersModuleAccess
			),
			users: users
				? users.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
		}),
		[
			locationTypes,
			departmentTypes,
			roleTypes,
			users,
			departmentsInfo,
			role,
			departmentId,
			departments,
			hasUsersModuleAccess,
		]
	);

	const departmentOptions = useMemo(
		() =>
			options.department.map((departmentOption) => ({
				...departmentOption,
				disabled:
					isHeadOrGroupManager(role) &&
					departmentId !== departmentOption.id &&
					!hasUsersModuleAccess,
			})),
		[options.department, departmentId, hasUsersModuleAccess]
	);

	const { position: allPositions } = options;

	const positionOptions = useMemo(
		() =>
			allPositions
				? allPositions.map((positionOption) => ({
						...positionOption,
						disabled: !availablePositions.includes(positionOption.name),
				  }))
				: [],
		[allPositions, availablePositions]
	);

	return {
		positionOptions,
		departmentOptions,
		defaultOptions: options,
	};
};
