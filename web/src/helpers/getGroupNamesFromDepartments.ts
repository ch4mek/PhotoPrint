import { isHeadOfDepartment, Roles } from '@/constants/roles';
import { DepartmentGroup, DepartmentResponse } from '@/types/department';

export const getGroupNamesFromDepartments = (
	departments: Array<DepartmentResponse>,
	role: Roles,
	userDepartmentFullName?: string,
	hasUsersModuleAccess?: boolean
): string[] => {
	const userDepartments =
		userDepartmentFullName && isHeadOfDepartment(role) && !hasUsersModuleAccess
			? departments.filter(
					({ departmentName }) => departmentName === userDepartmentFullName
			  )
			: departments;

	return Array.from(
		new Set(
			userDepartments
				.reduce(
					(allGroups: Array<DepartmentGroup>, { groups }) =>
						groups ? [...allGroups, ...groups] : allGroups,
					[]
				)
				.sort(({ id: leftId }, { id: rightId }) => leftId - rightId)
				.map(({ name }) => name)
		)
	);
};
