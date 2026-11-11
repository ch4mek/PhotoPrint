import { UserAccessClaims, UserClaims } from '@/types/claims';

export const checkIsHasClaims = (
	claimsArray: UserClaims[],
	userClaims: UserAccessClaims | null
) => {
	if (!userClaims) {
		return false;
	}

	return claimsArray.some(
		(claim) => claim in userClaims && userClaims[claim] !== undefined
	);
};
