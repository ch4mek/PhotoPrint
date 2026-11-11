import { ANY_USER_LABEL } from '../components/ApprovelsFieldsGroup/constants';

import { AcceptPolicesScope, AllRolesOptions } from '@/types/absences';

export const checkIsAnyUserRole = (
	role: string | null | undefined | AcceptPolicesScope
) => {
	if (!role) {
		return false;
	}

	return role === AllRolesOptions.AnyUser || role === ANY_USER_LABEL;
};
