import { UserClaims } from '@/types/claims';

export const INTERNSHIP_SECTION_TITLE = 'Internship';

export const internshipSectionRequiredClaims = [
	UserClaims.ManageInterns,
	UserClaims.ManageAllInterns,
	UserClaims.ManageHRModule,
];

export const INTERNSHIP_SECTION_CONTENT_MAX_HEIGHT = 326;
