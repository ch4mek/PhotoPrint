import { useMemo } from 'react';

import {
	SpecializationHardSkillRow,
	SpecializationSnapshotsByDates,
} from '@/pages/TeamBoardUser/types';
import { ShortHardSkillInfo } from '@/types/hardSkills';

export const useSpecializationHardSkillsTableRows = (
	snapshots: SpecializationSnapshotsByDates,
	allHardSkills: Array<ShortHardSkillInfo>,
	specializationId: number
): Array<SpecializationHardSkillRow> =>
	useMemo(() => {
		if (!allHardSkills || !snapshots) {
			return [];
		}

		return allHardSkills.map(({ name, id, isActive }) => ({
			id,
			name,
			snapshots,
			specializationId,
			isActive: !!isActive,
		}));
	}, []);
