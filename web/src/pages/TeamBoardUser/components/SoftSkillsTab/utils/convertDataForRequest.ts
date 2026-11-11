import { SeniorityFormsValues } from '@/forms/TeamboardForm/SeniorityForm/types';
import { RequestUserSoftSkill } from '@/queries/types';

export const convertDataForRequest = (
	values: SeniorityFormsValues
): RequestUserSoftSkill[] =>
	values.skills.map(({ softSkillId, seniority }) => ({
		seniorityLevelId: seniority?.id || '',
		softSkillId,
	}));
