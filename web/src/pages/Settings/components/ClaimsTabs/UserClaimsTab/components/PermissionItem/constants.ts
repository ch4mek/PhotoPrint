import { UserClaims } from '@/types/claims';

export const CLAIM_NAMES_EXCLUSIONS: Record<string, string> = {
	[UserClaims.ManageHRModule]: 'Manage HR Module',
};
