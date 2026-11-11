import { AbsenceAssignmentFormValues } from '@/forms/Absence/AbsenceAssignmentForm/types';

const fieldNames = {
	absence: 'absence',
	agreement: 'agreement',
	effectiveFromDate: 'effectiveFromDate',
	assignmentUsers: 'assignmentUsers',
};

export const initialFormValues: AbsenceAssignmentFormValues = {
	absence: null,
	agreement: null,
	effectiveFromDate: null,
};

export const formSections = {
	employees: 'Choose employees',
	absence: 'Choose absence',
};

export const absenceAssignmentFieldData = {
	absence: {
		name: fieldNames.absence,
		label: 'Absence',
		valueField: 'name',
	},
	agreement: {
		name: fieldNames.agreement,
		label: 'Agreement',
		valueField: 'name',
	},
	effectiveFromDate: {
		name: fieldNames.effectiveFromDate,
		innerLabel: 'Effective from',
	},
	assignmentUsers: {
		name: fieldNames.assignmentUsers,
	},
};

export const SEARCH_INPUT_DELAY = 300;
export const ASSIGNMENT_FORM_MOBILE_BREAKPOINT = 538;
export const ASSIGNMENT_FORM_TABLET_BREAKPOINT = 832;
export const ASSIGNMENT_FORM_TITLE = 'Absence assignment';
export const DISABLE_TOOLTIP_TEXT =
	'First you need to select users for assignment';
