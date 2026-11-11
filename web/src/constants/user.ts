import {
	EmailFieldName,
	GraduateJobPlacement,
	MilitaryServiceStatus,
} from '@/types/personalData';

export const DEFAULT_USER_COLOR = '#f7c531';

export const militaryServiceStatusOptions = Object.values(
	MilitaryServiceStatus
);
export const graduateJobPlacementOptions = Object.values(GraduateJobPlacement);

export const emailFormFields = {
	corporateEmail: {
		name: 'corporateEmail',
		label: 'Email (corporate)',
	},
	personalEmail: {
		name: 'personalEmail',
		label: 'Email (personal main)',
		validationLabel: 'Email address',
	},
	personalEmailSecond: {
		name: 'personalEmailSecond',
		label: 'Email (personal additional)',
		validationLabel: 'Email address',
	},
} as const;

export const EMAIL_FIELDS = [
	emailFormFields.corporateEmail.name,
	emailFormFields.personalEmail.name,
	emailFormFields.personalEmailSecond.name,
] as EmailFieldName[];

export const EMAIL_DUPLICATE_MESSAGE_PATTERN = /Email - (.+?) is already used/i;
