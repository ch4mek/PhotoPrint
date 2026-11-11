import { Roles } from '@/constants/roles';
import { RoleType } from '@/types/configuration';

export const filterAvailableRoles = (
	availableRoles: Array<RoleType>,
	position: Roles,
	roleRestrictions: Record<string, string[]>,
	shouldNotFilter?: boolean
) => {
	const restrictedRoles = roleRestrictions[position] || [];

	if (shouldNotFilter || !restrictedRoles.length) {
		return availableRoles;
	}

	return availableRoles.filter(({ id }) => !restrictedRoles.includes(id));
};
