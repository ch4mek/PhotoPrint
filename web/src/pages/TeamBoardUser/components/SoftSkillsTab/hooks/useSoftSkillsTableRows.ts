import { useMemo } from 'react';

import {
	SKELETON_SOFT_SKILL_ROW,
	SKELETON_SOFT_SKILL_ROW_AMOUNT,
} from '../constants';

import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { getSkillSnapshots } from '@/pages/TeamBoardUser/components/SoftSkillsTab/utils/getSkillSnapshots';
import { SoftSkillRow, UserSoftSkill } from '@/types/softSkills';

export const useSoftSkillsTableRows = (
	softSkills?: UserSoftSkill[],
	showSkeleton?: boolean
): SoftSkillRow[] =>
	useMemo(() => {
		if (showSkeleton) {
			return getSkeletonRows(
				SKELETON_SOFT_SKILL_ROW_AMOUNT,
				SKELETON_SOFT_SKILL_ROW
			);
		}

		if (!softSkills) {
			return [];
		}

		return softSkills.map((skill) => ({
			...skill,
			snapshots: getSkillSnapshots(skill),
		}));
	}, [softSkills, showSkeleton]);
