import { LookupUserResponse } from '@/types/userInfo';

export const expandUsersData = (
	usersList: Array<LookupUserResponse> | undefined,
	specificDepartment: string | null
) => {
	if (usersList) {
		if (specificDepartment) {
			return usersList.map((user) => ({
				...user,
				departmentId: specificDepartment,
			}));
		}

		return usersList;
	}

	return null;
};
