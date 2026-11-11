import { useMemo } from 'react';

import { AttendanceTotalOptions } from '../types';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { getFullName } from '@/helpers/utils';
import { DepartmentResponse } from '@/types/department';
import { LookupUserResponse } from '@/types/userInfo';

interface UseGridOptionsProps {
	departmentsInfo?: DepartmentResponse[];
	users?: LookupUserResponse[];
}

export const useGridOptions = ({
	departmentsInfo,
	users,
}: UseGridOptionsProps) => {
	const { role, departmentId } = useAuth();

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const options: AttendanceTotalOptions = useMemo(
		() => ({
			departamentsOptions: convertToSelectionModalOptions(
				departments,
				'fullName'
			),
			groupOptions: getGroupNamesFromDepartments(
				departmentsInfo ?? [],
				role,
				departmentTypes.find(
					({ id: currentDepartmentId }) => currentDepartmentId === departmentId
				)?.fullName
			),
			userOptions: users
				? users.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
		}),
		[departmentTypes, users, departmentsInfo, role, departmentId, departments]
	);

	const departmentOptions = useMemo(
		() =>
			options.departamentsOptions.map((departmentOption) => ({
				...departmentOption,
				disabled:
					isHeadOrGroupManager(role) && departmentId !== departmentOption.id,
			})),
		[options.departamentsOptions, departmentId]
	);

	return {
		departmentOptions,
		defaultOptions: options,
	};
};
