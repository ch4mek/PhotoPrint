import { SoftSkillSnapshotsByDates, UserSoftSkill } from '@/types/softSkills';

export const getSkillSnapshots = (
	skill: UserSoftSkill
): SoftSkillSnapshotsByDates =>
	skill.snapshots.reduce<SoftSkillSnapshotsByDates>(
		(acc, { softSkillSeniority, snapshotDate }) => {
			acc[snapshotDate] = {
				id: skill.id,
				softSkillName: skill.softSkillName,
				softSkillSeniority,
			};

			return acc;
		},
		{}
	);
