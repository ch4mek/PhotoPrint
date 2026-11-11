import { profitAndLossPageTabs } from '../constants';

import { filterTabsByClaims } from '@/helpers/filterTabsByClaims';
import { UserAccessClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

export const getProfitAndLossTabs = (
	userClaims: UserAccessClaims
): TabWithComponent[] => filterTabsByClaims(profitAndLossPageTabs, userClaims);
