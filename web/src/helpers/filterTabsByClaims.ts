import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserAccessClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

export const filterTabsByClaims = (
	tabs: TabWithComponent[],
	userClaims: UserAccessClaims | null
): TabWithComponent[] =>
	tabs.filter(
		({ claims }) => !claims?.length || checkIsHasClaims(claims, userClaims)
	);
