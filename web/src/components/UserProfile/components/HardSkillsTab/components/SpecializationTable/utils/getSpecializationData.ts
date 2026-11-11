import { SpecializationSnapshotsByDates } from '../../../types';

import { HardSkill, SpecializationSnapshot } from '@/types/userProfile';

export const getSpecializationData = (
	snapshots: SpecializationSnapshot[],
	specializationId: number
) =>
	snapshots.reduce<{
		snapshots: SpecializationSnapshotsByDates;
		hardSkills: HardSkill[];
	}>(
		(acc, snapshot) => {
			const { snapshotDate, userSpecializations } = snapshot;

			const specialization = userSpecializations.find(
				({ specializationTemplateId }) =>
					specializationId === specializationTemplateId
			);

			if (specialization) {
				acc.snapshots[snapshotDate] = specialization;

				specialization.hardSkills.forEach((skill) => {
					if (!acc.hardSkills.some(({ id }) => id === skill.id)) {
						acc.hardSkills.push(skill);
					}
				});
			}

			return acc;
		},
		{ snapshots: {}, hardSkills: [] }
	);
