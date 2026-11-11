import { isChiefRole, Roles } from '@/constants/roles';

export const getRoleRestrictions = (
	canAssignIntern: boolean,
	roleRestrictions: Partial<Record<Roles, Roles[]>>
) => {
	if (canAssignIntern) {
		return roleRestrictions;
	}

	return (Object.entries(roleRestrictions) as Array<[Roles, Roles[]]>).reduce<
		Partial<Record<Roles, Roles[]>>
	>((restrictions, [role, restrictedRoles]) => {
		restrictions[role] = isChiefRole(role)
			? restrictedRoles
			: [...restrictedRoles, Roles.IN];

		return restrictions;
	}, {});
};
