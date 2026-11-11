import { AutoselectOption } from '@/types/common';
import { InternshipSeniority } from '@/types/internship';

export type TechnologyHardSkillFormItem = {
	rowId: string;
	hardSkill: AutoselectOption | null;
	seniority: AutoselectOption<InternshipSeniority> | null;
};

export type TechnologyFormItem = {
	rowId: string;
	id: number | null;
	specialization: AutoselectOption | null;
	seniority: AutoselectOption<InternshipSeniority> | null;
	hardSkills: TechnologyHardSkillFormItem[];
};

export type TechnologiesFieldsValues = {
	technologies: TechnologyFormItem[];
};

export type SetFieldValue = (
	field: string,
	value: unknown,
	shouldValidate?: boolean
) => void;
