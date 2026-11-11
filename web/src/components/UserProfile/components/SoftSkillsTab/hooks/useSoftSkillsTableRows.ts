import { useMemo } from 'react';

import { getSkillSnapshots } from '@/pages/TeamBoardUser/components/SoftSkillsTab/utils/getSkillSnapshots';
import { SoftSkillRow, UserSoftSkill } from '@/types/softSkills';

export const useSoftSkillsTableRows = (
	softSkills?: UserSoftSkill[]
): SoftSkillRow[] =>
	useMemo(() => {
		if (!softSkills) {
			return [];
		}

		return softSkills.map((skill) => ({
			...skill,
			snapshots: getSkillSnapshots(skill),
		}));
	}, [softSkills]);
