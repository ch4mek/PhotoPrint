import {
	Roles,
	chiefRoles,
	isSysAdmin,
	isHeadOrGroupManager,
} from '@/constants/roles';
import { PersistentConfigurationItem } from '@/types/configuration';

export const getAvailablePositions = (
	allDepartments: PersistentConfigurationItem[] | undefined,
	userRole: Roles,
	userDepartmentId: null | undefined | string,
	hasUsersModuleAccess?: boolean
): PersistentConfigurationItem[] => {
	if (!allDepartments) {
		return [];
	}

	if (
		chiefRoles.includes(userRole) ||
		isSysAdmin(userRole) ||
		hasUsersModuleAccess
	) {
		return allDepartments;
	}

	const availablePositions = [Roles.HD, Roles.SE] as string[];

	if (isHeadOrGroupManager(userRole)) {
		availablePositions.push(Roles.GM);

		if (userDepartmentId === 'SysAdmin') {
			availablePositions.push(Roles.SA);
		}
	}

	return allDepartments.filter((department) =>
		availablePositions.includes(department.id as string)
	);
};
