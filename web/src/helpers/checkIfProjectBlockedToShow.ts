import { Roles } from '@/constants/roles';
import { ProjectEntity } from '@/types/project';

export const checkIfProjectBlockedToShow = (
	userRole: Roles,
	userDepartment: string,
	updatedProjectItem: ProjectEntity
) => {
	if (userRole !== Roles.HD || updatedProjectItem.isPublic) {
		return false;
	}

	return !updatedProjectItem.projectUsers
		.map((user) => user.departmentId)
		.find((department) => department === userDepartment);
};
