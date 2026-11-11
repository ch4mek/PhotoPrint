import { AllRolesOptions } from '@/types/absences';

export const MAX_NUMBER_OF_FIELDS = 6;

export const ANY_USER_LABEL = 'Any user';

export const roleOptions = [
	{ name: 'CEO/CTO', scope: AllRolesOptions.CEO_CTO },
	{ name: 'HoD', scope: AllRolesOptions.HOD },
	{ name: 'GM', scope: AllRolesOptions.GM },
	{ name: 'PM', scope: AllRolesOptions.PM },
	{ name: ANY_USER_LABEL, scope: AllRolesOptions.AnyUser },
];

export const initialAddMoreFieldValue = { scope: '', userId: null };
