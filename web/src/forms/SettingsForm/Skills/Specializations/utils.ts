import { Theme } from '@emotion/react';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

import {
	DEPARMENT_DOESNT_BELONG,
	DEPARTMENT_HAS_ASSIGNED_USERS,
} from './constants';

export const createDepartmentDisabledMessage = (
	isOptionHasAssignedUsers: boolean,
	hasAccessToDepartment: boolean
) => {
	if (!hasAccessToDepartment) {
		return DEPARMENT_DOESNT_BELONG;
	}

	if (isOptionHasAssignedUsers) {
		return DEPARTMENT_HAS_ASSIGNED_USERS;
	}

	return undefined;
};

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
		hasContols: true,
	}),
});
