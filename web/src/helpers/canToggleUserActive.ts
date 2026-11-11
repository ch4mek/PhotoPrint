import { roleActivationRules, Roles } from '@/constants/roles';

type CanToggleUserActiveParams = {
	currentUserRole: Roles;
	targetUserRole: Roles;
	currentUserId: number;
	targetUserId: number;
	hasHrClaim: boolean;
	currentUserDepartmentId?: string;
	targetUserDepartmentId?: string;
};

export const canToggleUserActive = ({
	currentUserRole,
	targetUserRole,
	currentUserId,
	targetUserId,
	currentUserDepartmentId,
	targetUserDepartmentId,
	hasHrClaim,
}: CanToggleUserActiveParams) => {
	if (currentUserId === targetUserId) {
		return false;
	}

	const isSameDepartment =
		currentUserDepartmentId &&
		targetUserDepartmentId &&
		currentUserDepartmentId === targetUserDepartmentId;

	if (hasHrClaim) {
		return true;
	}

	return roleActivationRules[currentUserRole](
		targetUserRole,
		!!isSameDepartment
	);
};
