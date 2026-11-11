import { ShortAssignedSoftSkillInfo } from '@/forms/TeamboardForm/SeniorityForm/types';
import { AssignedUserSoftSkill } from '@/types/teamboard';

export const convertResponseToFormData = (
	assignedSkills?: AssignedUserSoftSkill[]
): ShortAssignedSoftSkillInfo[] => {
	if (!assignedSkills) {
		return [];
	}

	return assignedSkills.map(
		({ softSkillId, softSkillName, seniorityLevelName, seniorityLevelId }) => ({
			softSkillId,
			softSkillName,
			seniority: {
				id: seniorityLevelId ?? 'None',
				name: seniorityLevelName ?? 'None',
			},
		})
	);
};
