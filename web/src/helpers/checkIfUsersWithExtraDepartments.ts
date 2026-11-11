import { UserOnProject } from '@/types/userInfo';

export const checkIfUsersWithExtraDepartments = (
	usersList: Array<UserOnProject> | undefined,
	departmentId: string | null
) => {
	if (!usersList || usersList.length === 0 || !departmentId) {
		return false;
	}

	if (usersList.find((user) => user.departmentId !== departmentId)) {
		return true;
	}

	return false;
};
