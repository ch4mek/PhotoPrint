import type { NullableRole } from '@/types/roles';

import { extraFiltersBy } from './filters';

export enum Roles {
	CEO = 'CEO',
	CTO = 'CTO',
	CLO = 'CLO',
	GM = 'GroupManager',
	HD = 'HeadOfDepartment',
	SE = 'Employee',
	SA = 'SystemAdministrator',
	IN = 'Intern',
}

export const AllRoles = [
	Roles.CEO,
	Roles.CTO,
	Roles.CLO,
	Roles.SE,
	Roles.HD,
	Roles.SA,
	Roles.GM,
	Roles.IN,
];
export const chiefRoles = [Roles.CEO, Roles.CTO, Roles.CLO];
export const employees = [Roles.SA, Roles.SE];
export const managerRoles = [...chiefRoles, Roles.HD];
export const chiefsAndSA = [...chiefRoles, Roles.SA];
export const allRolesExceptIntern = AllRoles.filter(
	(role) => role !== Roles.IN
);

export const groupManagerAndHead = [Roles.GM, Roles.HD];
export const managerRolesWithGroupManager = [...managerRoles, Roles.GM];
export const managerRolesWithGroupManagerAndSA = [
	...managerRolesWithGroupManager,
	Roles.SA,
];
export const employeeAndInternRoles = [Roles.SE, Roles.IN];

export const isNotChiefRole = (role?: NullableRole) =>
	role != null && !chiefRoles.includes(role);
export const isChiefRole = (role?: NullableRole) =>
	role != null && chiefRoles.includes(role);
export const isManagerRole = (role?: NullableRole) =>
	role != null && managerRoles.includes(role);
export const isRegularEmployee = (role?: NullableRole) =>
	role != null && employees.includes(role);
export const isHeadOfDepartment = (role?: NullableRole) => role === Roles.HD;
export const isCEO = (role?: NullableRole) => role === Roles.CEO;
export const isCTO = (role?: NullableRole) => role === Roles.CTO;
export const isCLO = (role?: NullableRole) => role === Roles.CLO;
export const isEmployee = (role?: NullableRole) => role === Roles.SE;
export const isSysAdmin = (role?: NullableRole) => role === Roles.SA;
export const isGroupManager = (role?: NullableRole) => role === Roles.GM;
export const isIntern = (role?: NullableRole) => role === Roles.IN;

export const isHeadOrGroupManager = (role?: NullableRole) =>
	role != null && groupManagerAndHead.includes(role);
export const isManagerRoleOrGroupManager = (role?: NullableRole) =>
	role != null && managerRolesWithGroupManager.includes(role);

export const isRoleWithGroup = (userRole?: NullableRole) =>
	isGroupManager(userRole) ||
	(userRole != null && employeeAndInternRoles.includes(userRole));

export const RoleExtraOptions = {
	...extraFiltersBy,
	AllUsers: 'All Users',
};

export const roleActivationPriority: Record<
	Exclude<Roles, Roles.CLO>,
	number
> = {
	[Roles.CEO]: 6,
	[Roles.CTO]: 5,
	[Roles.HD]: 4,
	[Roles.GM]: 3,
	[Roles.SA]: 2,
	[Roles.SE]: 1,
	[Roles.IN]: 0,
};

export const roleActivationRules: Record<
	Exclude<Roles, Roles.CLO>,
	(target: Exclude<Roles, Roles.CLO>, sameDepartment: boolean) => boolean
> = {
	[Roles.CEO]: (target) => target !== Roles.CEO,
	[Roles.CTO]: (target) => ![Roles.CEO, Roles.CTO].includes(target),
	[Roles.HD]: (target, sameDepartment) =>
		sameDepartment &&
		roleActivationPriority[Roles.HD] > roleActivationPriority[target],
	[Roles.GM]: () => false,
	[Roles.SA]: (target) => target === Roles.SE,
	[Roles.SE]: () => false,
	[Roles.IN]: () => false,
};
