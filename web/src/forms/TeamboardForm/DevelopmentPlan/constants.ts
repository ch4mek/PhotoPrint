import { DevelopmentPlanFormField, DevelopmentPlanFormValues } from './types';

export const developmentPlanFormInitialValues: DevelopmentPlanFormValues = {
	startDate: null,
	deadlineDate: null,
	summary: '',
	notes: '',
};

export const developmentPlanFormFields = {
	startDate: {
		innerLabel: 'Start date',
	},
	deadlineDate: {
		innerLabel: 'Deadline date',
	},
	summary: {
		name: DevelopmentPlanFormField.Summary,
		label: 'Summary',
	},
	notes: {
		name: DevelopmentPlanFormField.Notes,
		label: 'Notes',
	},
};

export const FORM_NAME = 'Teamboard devplan form';
