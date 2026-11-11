import {
	Roles,
	isGroupManager,
	isHeadOfDepartment,
	isChiefRole,
} from '@/constants/roles';
import { PersonalDataResponse } from '@/types/personalData';

interface UsersParameters {
	currentUserRole: Roles;
	currentUserDepartmentId: string | null;
	currentUserGroupId: number | string | null;
	pickedUserRole: PersonalDataResponse['position'];
	pickedUserDepartmentId: PersonalDataResponse['departmentId'];
	pickedUserGroupId: PersonalDataResponse['resourceGroupId'];
}

const isSameResourceGroupId = (
	firstGroupId: number | string | null,
	secondGroupId: number | string | null
) =>
	firstGroupId != null &&
	secondGroupId != null &&
	String(firstGroupId) === String(secondGroupId);

export const isCurrentUserManagerOfPickedUser = ({
	currentUserRole,
	pickedUserRole,
	currentUserDepartmentId,
	pickedUserDepartmentId,
	currentUserGroupId,
	pickedUserGroupId,
}: UsersParameters) => {
	if (currentUserRole === pickedUserRole) {
		return false;
	}

	if (isChiefRole(currentUserRole)) {
		return !isChiefRole(pickedUserRole);
	}

	const isSameDepartment = currentUserDepartmentId
		? currentUserDepartmentId === pickedUserDepartmentId
		: false;

	if (isHeadOfDepartment(currentUserRole)) {
		return isSameDepartment;
	}

	if (isGroupManager(currentUserRole) && currentUserGroupId) {
		return (
			isSameDepartment &&
			isSameResourceGroupId(currentUserGroupId, pickedUserGroupId)
		);
	}

	return false;
};
