import { WORDS_BY_UPPERCASE_OR_LOWERCASE_LETTER } from '@/constants/regexp';
import {
	isChiefRole,
	isRegularEmployee,
	isGroupManager,
	Roles,
} from '@/constants/roles';
import { CALENDAR_TAB_ID } from '@/constants/settings';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { filterTabsByClaims } from '@/helpers/filterTabsByClaims';
import { UserAccessClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

import {
	ABSENCES_TAB_ID,
	commonSettingsPageTabs,
	settingsAbsencesTab,
	settingsPageTabs,
} from './constants';

export const splitValueByCapitalLetters = (claimName: string) =>
	claimName.split(WORDS_BY_UPPERCASE_OR_LOWERCASE_LETTER).join(' ');

const getFilteredSettingsTabs = (
	tabs: TabWithComponent[],
	userClaims: UserAccessClaims | null
) =>
	filterTabsByClaims(tabs, userClaims).sort(
		({ id: firstTabId }, { id: secondTabId }) => firstTabId - secondTabId
	);

export const getSettingsTabs = (role: Roles, userClaims: UserAccessClaims) => {
	if (isRegularEmployee(role)) {
		return getFilteredSettingsTabs(settingsAbsencesTab, userClaims);
	}

	if (isGroupManager(role)) {
		return commonSettingsPageTabs.filter(({ id, claims }) => {
			if (id === CALENDAR_TAB_ID) {
				return true;
			}

			if (id === ABSENCES_TAB_ID) {
				return checkIsHasClaims(claims, userClaims);
			}

			return false;
		});
	}

	const tabs = isChiefRole(role) ? settingsPageTabs : commonSettingsPageTabs;

	return getFilteredSettingsTabs(tabs, userClaims);
};
