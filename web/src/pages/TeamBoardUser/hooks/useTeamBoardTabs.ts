import { teamBoardUserTabs } from '../constants';
import {
	TeamBoardTabsId,
	TeamboardUserAccessLevel,
	TeamboardUserProfileKind,
} from '../types';
import { filterTabsByProfileKind, hasAccess } from '../utils';

import { isGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';

export const useTeamBoardTabs = (
	accessLevel: TeamboardUserAccessLevel,
	profileKind: TeamboardUserProfileKind
) => {
	const userInfo = useAuth();
	const { role, claims } = userInfo;

	const isGroupManagerRole = isGroupManager(role);

	const updatedTabs = teamBoardUserTabs.filter(
		(tab) => tab.id !== TeamBoardTabsId.SALARY_DETAILS
	);

	const commonTeamBoardTabs = isGroupManagerRole
		? updatedTabs
		: teamBoardUserTabs;

	return filterTabsByProfileKind(
		commonTeamBoardTabs.filter(
			(tab) =>
				hasAccess(accessLevel, tab.accessLevels) &&
				(!tab.requiredClaims || checkIsHasClaims(tab.requiredClaims, claims))
		),
		profileKind
	);
};
