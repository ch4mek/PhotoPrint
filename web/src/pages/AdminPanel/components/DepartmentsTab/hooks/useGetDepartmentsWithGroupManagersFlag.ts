import { useCallback, useMemo } from 'react';

import {
	DEPARTMENT_GROUP_MANAGERS_QUERY_ORDER_KEY,
	DEPARTMENT_GROUP_MANAGERS_QUERY_PAGE,
	DEPARTMENT_GROUP_MANAGERS_QUERY_SORT_DIRECTION,
	DEPARTMENT_GROUP_MANAGERS_QUERY_TOP,
	departmentGroupManagersQueryParams,
} from '../constants';
import { DepartmentWithGroupManagersFlag } from '../types';

import { useConfiguration } from '@/context/Configuration';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useUsers } from '@/queries/useUsers';

export function useGetDepartmentsWithGroupManagersFlag() {
	const {
		configurableValues: { departmentTypes, roleTypes },
		persistentValues: { locationTypes },
	} = useConfiguration();
	const {
		data: departments,
		isLoading: isDepartmentsLoading,
		isFetching: isDepartmentsFetching,
		refetch: refetchDepartmens,
	} = useGetDepartments();
	const {
		data: users,
		isLoading: isUsersLoading,
		isFetching: isUsersFetching,
		refetch: refetchUsers,
	} = useUsers(
		DEPARTMENT_GROUP_MANAGERS_QUERY_PAGE,
		DEPARTMENT_GROUP_MANAGERS_QUERY_TOP,
		DEPARTMENT_GROUP_MANAGERS_QUERY_SORT_DIRECTION,
		DEPARTMENT_GROUP_MANAGERS_QUERY_ORDER_KEY,
		departmentGroupManagersQueryParams,
		departmentTypes,
		locationTypes,
		roleTypes
	);

	const isDataLoading =
		isDepartmentsLoading ||
		isDepartmentsFetching ||
		isUsersLoading ||
		isUsersFetching;

	const refetch = useCallback(() => {
		refetchUsers();
		refetchDepartmens();
	}, []);

	const mappedDepartments = useMemo<DepartmentWithGroupManagersFlag[]>(() => {
		if (!departments || !users?.items) {
			return [];
		}

		return departments.map<DepartmentWithGroupManagersFlag>(
			({ departmentName, ...restDepartmentProps }) => {
				const departmentId = departmentTypes.find(
					({ fullName }) => fullName === departmentName
				)?.id;
				const departmentGroupManager = users.items.find(
					(user) => user.departmentId === departmentId
				);

				return {
					id: departmentId,
					departmentName,
					hasGroupManagers: Boolean(departmentGroupManager),
					...restDepartmentProps,
				};
			}
		);
	}, [departments, users]);

	return { data: mappedDepartments, refetch, isLoading: isDataLoading };
}
