import { InternshipFinalResult, InternshipType } from '@/types/labboard';

import { createDefaultTechnologyItem } from './components/TechnologiesFields/constants';
import { InternshipFormFields, InternshipFormValues } from './types';

export const POPUP_TITLE = 'Internship editing';
export const POPUP_MAX_WIDTH = 640;

export const internshipTypeOptions = Object.values(InternshipType);
export const internshipFinalResultOptions = Object.values(
	InternshipFinalResult
);

export const internshipFormFields = {
	internshipSpecialization: {
		name: InternshipFormFields.Specialization,
		label: 'Internship specialization',
	},
	internshipType: {
		name: InternshipFormFields.Type,
		label: 'Type',
	},
	internshipEngineerStage: {
		name: InternshipFormFields.EngineerStage,
		label: 'Engineer stage',
	},
	internshipFinalResult: {
		name: InternshipFormFields.FinalResult,
		label: 'Final result',
	},
	internshipCompletedPoints: {
		name: InternshipFormFields.CompletedPoints,
		label: 'Completed points',
	},
	internshipApprovedPoints: {
		name: InternshipFormFields.ApprovedPoints,
		label: 'Approved points',
	},
	internshipStartDate: {
		name: InternshipFormFields.StartDate,
		label: 'Internship start date',
	},
	internshipPlannedEndDate: {
		name: InternshipFormFields.PlannedEndDate,
		label: 'Planned internship end date',
	},
} as const;

export const emptyInitialValues: InternshipFormValues = {
	id: null,
	internshipSpecialization: '',
	internshipType: null,
	internshipEngineerStage: '',
	internshipFinalResult: null,
	internshipCompletedPoints: '',
	internshipApprovedPoints: '',
	internshipStartDate: null,
	internshipPlannedEndDate: null,
	technologies: [createDefaultTechnologyItem()],
};
