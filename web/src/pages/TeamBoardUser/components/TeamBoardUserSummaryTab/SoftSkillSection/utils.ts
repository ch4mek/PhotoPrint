import { SoftSkillRow } from '@/types/softSkills';
import { AssignedUserSoftSkill } from '@/types/teamboard';

export const mapSoftSkillsLookupToRows = (
	items: Array<AssignedUserSoftSkill>
): Array<SoftSkillRow> =>
	items.map(({ softSkillId, softSkillName, seniorityLevelName }) => ({
		id: softSkillId,
		snapshots: {
			id: softSkillId,
			softSkillName,
			softSkillSeniority: seniorityLevelName,
		},
	})) as unknown as SoftSkillRow[];
