import { useMemo } from 'react';

import { SoftSkillsSnapshotDates, UserSoftSkill } from '@/types/softSkills';

export const useGetAllSnapshotsDates = (softSkills?: UserSoftSkill[]) =>
	useMemo(() => {
		if (!softSkills) {
			return [];
		}

		const seenDates = new Set<string>();

		return softSkills
			.reduce<SoftSkillsSnapshotDates[]>((acc, { snapshots }) => {
				snapshots.forEach(({ snapshotDate, snapshotId }) => {
					if (!seenDates.has(snapshotDate)) {
						seenDates.add(snapshotDate);
						acc.push({ snapshotDate, snapshotId });
					}
				});

				return acc;
			}, [])
			.sort((a, b) => b.snapshotDate.localeCompare(a.snapshotDate));
	}, [softSkills]);
