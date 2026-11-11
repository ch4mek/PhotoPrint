import {
	chiefRoles,
	managerRolesWithGroupManagerAndSA,
	Roles,
} from '@/constants/roles';

export const formSections = {
	personalDetails: 'Personal details',
	contacts: 'Contacts',
	workDetails: 'Work details',
	contractDetails: 'Contract details',
	location: 'Location',
};

export const ENTITY_NAME = 'User';

const { CEO, CTO, SA, SE, GM, HD } = Roles;

export const roleRestrictions: Partial<Record<Roles, Roles[]>> = {
	[CEO]: chiefRoles,
	[CTO]: chiefRoles,
	[SA]: managerRolesWithGroupManagerAndSA,
	[SE]: managerRolesWithGroupManagerAndSA,
	[GM]: managerRolesWithGroupManagerAndSA,
	[HD]: managerRolesWithGroupManagerAndSA,
};

export const DEPARTMENT_DISABLED_TOOLTIP = "You can't choose user's department";
