import { AutoselectOption } from '@/types/common';
import { InternshipSeniority } from '@/types/internship';

import { TechnologyFormItem, TechnologyHardSkillFormItem } from './types';

export const TECHNOLOGIES_FIELD_NAME = 'technologies';
export const ADD_SPECIALIZATION_BUTTON_TITLE = '+ Add specialization';
export const ADD_HARD_SKILL_BUTTON_TITLE = '+ Add hard skill';
export const MAX_SPECIALIZATIONS = 10;
export const MAX_HARD_SKILLS_PER_SPECIALIZATION = 20;
export const REMOVE_SPECIALIZATION_ARIA_LABEL = 'Remove specialization';
export const REMOVE_HARD_SKILL_ARIA_LABEL = 'Remove hard skill';

export const internshipSeniorityOptions: AutoselectOption<InternshipSeniority>[] =
	Object.values(InternshipSeniority).map((seniority) => ({
		id: seniority,
		name: seniority,
	}));

export const TECHNOLOGIES_FIELDS_DATA = {
	specialization: {
		name: 'specialization',
		label: 'Specialization',
		valueField: 'name',
	},
	specializationSeniority: {
		name: 'seniority',
		label: 'Seniority',
		valueField: 'name',
	},
	hardSkill: {
		name: 'hardSkill',
		label: 'Hard skill',
		valueField: 'name',
	},
	hardSkillSeniority: {
		name: 'seniority',
		label: 'Seniority',
		valueField: 'name',
	},
} as const;

const createRowId = () => crypto.randomUUID();

export const createDefaultHardSkillItem = (): TechnologyHardSkillFormItem => ({
	rowId: createRowId(),
	hardSkill: null,
	seniority: null,
});

export const createDefaultTechnologyItem = (): TechnologyFormItem => ({
	rowId: createRowId(),
	id: null,
	specialization: null,
	seniority: null,
	hardSkills: [],
});
