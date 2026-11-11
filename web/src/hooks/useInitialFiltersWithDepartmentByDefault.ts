import { useMemo } from 'react';

import { isHeadOrGroupManager, Roles } from '@/constants/roles';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserAccessClaims, UserClaims } from '@/types/claims';

export const useInitialFiltersWithDepartmentByDefault = <
	T extends { departments: string[] }
>(
	initialFilters: T,
	role: Roles,
	departmentId: string,
	claims?: UserAccessClaims,
	exemptClaims?: UserClaims[]
) =>
	useMemo(() => {
		const hasExemptClaim =
			exemptClaims && claims ? checkIsHasClaims(exemptClaims, claims) : false;

		return {
			...initialFilters,
			departments:
				isHeadOrGroupManager(role) && !hasExemptClaim ? [departmentId] : [],
		};
	}, [role, departmentId, claims, exemptClaims]);
