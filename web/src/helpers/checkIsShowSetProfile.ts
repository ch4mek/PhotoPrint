import { chiefRoles, Roles } from '@/constants/roles';

export const checkIsShowSetProfile: (
	location: string,
	departmentId: string,
	position: Roles
) => boolean = (location, departmentId, position) => {
	const isShowSetProfile = chiefRoles.includes(position)
		? !location
		: !location || !departmentId;

	return isShowSetProfile;
};
