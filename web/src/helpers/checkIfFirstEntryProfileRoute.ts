import { chiefRoles } from '@/constants/roles';
import { UserInfo } from '@/types/userInfo';

export const checkIfFirstEntryProfileRoute = (
	userInfo: UserInfo,
	authorized: boolean
) =>
	!(
		userInfo.location &&
		(userInfo.departmentId || chiefRoles.includes(userInfo.position))
	) && authorized;
