import {
	Roles,
	isManagerRoleOrGroupManager,
	isSysAdmin,
} from '@/constants/roles';

import {
	adminPanelPageTabs,
	systemAdministratorAdminPanelPageTabs,
	HRsAdminPanelPageTabs,
} from './constants';

export const getTabs = (role: Roles) => {
	if (isManagerRoleOrGroupManager(role)) {
		return adminPanelPageTabs.sort(
			({ id: firstId }, { id: secondId }) => firstId - secondId
		);
	}

	if (isSysAdmin(role)) {
		return systemAdministratorAdminPanelPageTabs;
	}

	return HRsAdminPanelPageTabs;
};
