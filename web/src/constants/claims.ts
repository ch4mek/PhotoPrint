import { UserClaims } from '@/types/claims';

export const personalInfoClaimAccess = [
	UserClaims.ManageHRModule,
	UserClaims.ManageAccountant,
	UserClaims.ManageProjectCoordinator,
];

export const personalInfoPopupUrlClaims = [
	...personalInfoClaimAccess,
	UserClaims.ManageAllInterns,
];
