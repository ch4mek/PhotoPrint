import { Dayjs } from '@/lib/dayjs';
import { SeniorityLevelType } from '@/types/configuration';

export interface SeniorityFormProps {
	handleSubmit: (data: SeniorityFormsValues) => void;
	handleClose: VoidFunction;
	snapshotDate?: string;
	userId?: number;
	editSnapshotId?: number | string;
}
export interface SeniorityFormsValues {
	date: Dayjs;
	skills: ShortAssignedSoftSkillInfo[];
}

export interface ShortAssignedSoftSkillInfo {
	softSkillId: number;
	softSkillName: string;
	seniority: SeniorityLevelType | null;
}

export interface SkillsTouched {
	skills?: boolean[];
}
