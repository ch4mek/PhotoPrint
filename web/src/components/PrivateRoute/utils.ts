import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserClaims } from '@/types/claims';

export const checkIsRedirect = (
	available: Roles[] | undefined,
	availablePermissions: UserClaims[] | undefined
) => {
	const { role, claims } = useAuth();

	const isRoleAvailable = available ? available.includes(role) : false;
	const hasPermissions = availablePermissions
		? checkIsHasClaims(availablePermissions, claims)
		: false;

	return !(isRoleAvailable || hasPermissions);
};
