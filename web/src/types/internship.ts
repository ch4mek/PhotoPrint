import { FormAutoselectOption } from '@/types/forms';
import { InternshipFinalResult, InternshipType } from '@/types/labboard';

export enum InternshipSeniority {
	Trainee = 'Trainee',
	Junior = 'Junior',
	Middle = 'Middle',
	Senior = 'Senior',
	Lead = 'Lead',
	Architect = 'Architect',
}

export interface InternshipHardSkill {
	id: number;
	name: string;
	seniorityLevelId: string | null;
	seniorityLevelName: string | null;
}

export interface InternshipTechnology {
	id: number;
	specializationTemplateId: number;
	specializationTemplateName: string;
	seniorityLevelId: string | null;
	seniorityLevelName: string | null;
	hardSkills: InternshipHardSkill[];
}

export type InternshipTechnologyPayload = Omit<InternshipTechnology, 'id'> & {
	id: number | null;
};

export interface InternshipResponse {
	id: number;
	userId: number;
	specialization: string;
	type: InternshipType;
	engineerStage: number;
	completedPoints: number | null;
	approvedPoints: number | null;
	finalResult: InternshipFinalResult | null;
	startDate: string;
	plannedEndDate: string;
	userInternshipSpecializations: InternshipTechnology[];
}

export interface UpdateInternshipPayload {
	id: number | null;
	specialization: string;
	type: InternshipType;
	engineerStage: number;
	completedPoints: number | null;
	approvedPoints: number | null;
	finalResult: InternshipFinalResult | null;
	startDate: string | null;
	plannedEndDate: string | null;
	internshipSpecializations: InternshipTechnologyPayload[];
}

export interface InternshipSpecializationLookupItem {
	specializationTemplateId: number;
	specializationTemplateName: string;
	hardSkills: Array<FormAutoselectOption>;
}
