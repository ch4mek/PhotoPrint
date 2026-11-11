import { FormikErrors } from 'formik';

import { ShortAssignedSoftSkillInfo } from '@/forms/TeamboardForm/SeniorityForm/types';
import { SeniorityLevelType } from '@/types/configuration';

export interface SkillsListProps {
	skills: ShortAssignedSoftSkillInfo[];
	onSeniorityChange: (
		index: number
	) => (newSeniority: SeniorityLevelType | null) => void;
	errors?: FormikErrors<ShortAssignedSoftSkillInfo[]>;
	touched?: boolean[];
}
