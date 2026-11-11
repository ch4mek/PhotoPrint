import { isChiefRole, Roles } from '@/constants/roles';

export const personalInfoPopupAccessRules = {
	notOwnProfile: (userId: number, currentUserId: number) =>
		userId !== currentUserId,
	canOpenByRole: (targetPosition: Roles, hasPersonalInfoAccess: boolean) =>
		hasPersonalInfoAccess || !isChiefRole(targetPosition),
};
