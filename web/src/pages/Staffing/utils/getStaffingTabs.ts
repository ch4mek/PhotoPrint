import { staffingPageTabs } from '../constants';

import { filterTabsByClaims } from '@/helpers/filterTabsByClaims';
import { UserAccessClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

export const getStaffingTabs = (
	userClaims: UserAccessClaims
): TabWithComponent[] => filterTabsByClaims(staffingPageTabs, userClaims);
