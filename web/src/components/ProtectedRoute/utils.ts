import { UserClaims, ClaimAccessType } from '@/types/claims';
import { DecodedToken } from '@/types/common';

export function extractClaims(decodedToken: DecodedToken) {
	const claims: Record<keyof typeof UserClaims, ClaimAccessType> = {} as Record<
		keyof typeof UserClaims,
		ClaimAccessType
	>;

	Object.keys(UserClaims).forEach((claim) => {
		const claimKey = UserClaims[claim as keyof typeof UserClaims];

		if (decodedToken[claimKey] !== undefined) {
			claims[claimKey] = decodedToken[claimKey];
		}
	});

	return claims;
}
