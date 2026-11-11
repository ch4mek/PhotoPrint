import { InternshipHardSkill } from '@/types/internship';

export const formatNameWithSeniority = (
	name: string,
	seniority?: string | null
) => (seniority ? `${name} (${seniority})` : name);

export const formatHardSkillsText = (
	hardSkills: InternshipHardSkill[],
	fallbackSeniority?: string | null
) => {
	if (!hardSkills.length) {
		return '-';
	}

	return hardSkills
		.map(({ name, seniorityLevelName }) =>
			formatNameWithSeniority(name, seniorityLevelName ?? fallbackSeniority)
		)
		.join(', ');
};
