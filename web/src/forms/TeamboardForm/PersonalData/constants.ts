import { chiefRoles, Roles } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import { emailFormFields } from '@/constants/user';
import { ContractsFilters } from '@/pages/TeamBoardUser/components/ContractsTab/types';

export const PERSONAL_DATA_FORM_FIELDS = {
	lastName: {
		name: 'lastName',
		label: 'Last name (english)',
	},
	firstName: {
		name: 'firstName',
		label: 'First name (english)',
	},
	lastNameRu: {
		name: 'lastNameRu',
		label: 'Last name (russian)',
	},
	firstNameRu: {
		name: 'firstNameRu',
		label: 'First name (russian)',
	},
	patronymicRu: {
		name: 'patronymicRu',
		label: 'Middle name (russian)',
	},
	dateOfBirth: {
		name: 'dateOfBirth',
		label: 'Date of birth',
	},
	position: {
		name: 'position',
		label: 'Position',
	},
	department: {
		name: 'department',
		label: 'Department',
		isRequired: true,
	},
	group: {
		name: 'group',
		label: 'Group',
	},
	...emailFormFields,
	personalPhone: {
		name: 'personalPhone',
		label: 'Phone number',
	},
	englishLevel: {
		name: 'englishLevel',
		label: 'English level',
		valueField: 'name',
	},
	telegram: {
		name: 'telegram',
		label: 'Telegram',
	},
	registeredLocation: {
		name: 'registeredLocation',
		label: 'Registered location',
	},
	location: {
		name: 'location',
		label: 'Location',
	},
	militaryServiceStatus: {
		name: 'militaryServiceStatus',
		label: 'Military service status',
	},
	graduateJobPlacement: {
		name: 'graduateJobPlacement',
		label: 'Graduate job placement',
	},
	linkedIn: {
		name: 'linkedIn',
		label: 'LinkedIn',
	},
};

const { CEO, CTO, GM, HD, SA, SE } = Roles;

export const roleRestrictions: Record<string, string[]> = {
	[CEO]: chiefRoles,
	[CTO]: chiefRoles,
	[SA]: chiefRoles,
	[SE]: chiefRoles,
	[GM]: [...chiefRoles, GM, HD, SA],
	[HD]: [...chiefRoles, HD, SA],
};

export const POPUP_TITLE = 'Personal data editing';
export const FORM_NAME = 'Teamboard personal data form';

export const DEPARTMENT_FIELD_DISABLED_TEXT =
	'You can’t choose user’s department';

export const GROUP_FIELD_DISABLED_TEXT = 'You can’t choose user’s group';

export const CONTRACTS_SKIP = 0;
export const CONTRACTS_TOP = 1;
export const CONTRACTS_SORT_ORDER = SortDirection.DESC;
export const CONTRACTS_SORT_BY = 'contractFrom';
export const EMPTY_CONTRACTS_FILTER = {} as ContractsFilters;
