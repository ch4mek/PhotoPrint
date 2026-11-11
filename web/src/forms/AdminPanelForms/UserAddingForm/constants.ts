import { emailFormFields } from '@/constants/user';

import { AdminPanelUserAddingFormValues } from './types';

export const FORM_NAME = 'userAddingForm';

export const ADMIN_PANEL_USER_INITIAL_VALUES: AdminPanelUserAddingFormValues = {
	firstName: '',
	lastName: '',
	firstNameRu: '',
	lastNameRu: '',
	patronymicRu: '',
	dateOfBirth: null,
	militaryServiceStatus: null,

	personalPhone: '',
	telegram: '',
	personalEmail: '',
	personalEmailSecond: '',
	linkedIn: '',

	position: null,
	department: null,
	group: null,
	corporateEmail: '',

	hireDate: null,
	contractStartDate: null,
	contractEndDate: null,
	graduateJobPlacement: null,

	currentLocation: '',
	officeLocation: null,
	registeredLocation: null,
};

export const ADMIN_PANEL_USER_FORM_FIELDS = {
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
	militaryServiceStatus: {
		name: 'militaryServiceStatus',
		label: 'Military service status',
	},

	personalPhone: {
		name: 'personalPhone',
		label: 'Phone number',
	},
	telegram: {
		name: 'telegram',
		label: 'Telegram',
	},
	...emailFormFields,
	linkedIn: {
		name: 'linkedIn',
		label: 'LinkedIn',
	},

	position: {
		name: 'position',
		label: 'Position',
	},
	department: {
		name: 'department',
		label: 'Department',
	},
	group: {
		name: 'group',
		label: 'Group',
	},

	hireDate: {
		name: 'hireDate',
		label: 'Hire date',
	},
	contractStartDate: {
		name: 'contractStartDate',
		label: 'Contract start date',
	},
	contractEndDate: {
		name: 'contractEndDate',
		label: 'Contract end date',
	},
	graduateJobPlacement: {
		name: 'graduateJobPlacement',
		label: 'Graduate job placement',
	},

	currentLocation: {
		name: 'currentLocation',
		label: 'Current location',
	},
	officeLocation: {
		name: 'officeLocation',
		label: 'Office location',
	},
	registeredLocation: {
		name: 'registeredLocation',
		label: 'Registered location',
	},
};
