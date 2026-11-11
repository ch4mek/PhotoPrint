import { ClaimsRowConfig } from '../types';
import { CLAIM_COLUMN_WIDTH } from '../UserClaimsTab/components/UserClaimsGrid/constants';

export const createUsersObjectConfig = (
	claims: string[]
): Record<string, ClaimsRowConfig> =>
	claims.reduce((acc, claim) => {
		acc[claim] = {
			name: claim,
			width: CLAIM_COLUMN_WIDTH,
			key: claim,
		};

		return acc;
	}, {} as Record<string, ClaimsRowConfig>);
