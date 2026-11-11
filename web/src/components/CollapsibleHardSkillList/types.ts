import { FormikErrors } from 'formik';

import { SeniorityLevelType } from '@/types/configuration';
import { FormFullHardSkillInfo } from '@/types/teamboard';

export interface SpecializationHardSkillListProps {
	onSpecializationSeniorityChange: (
		hardSkillId: number
	) => (newSeniority: SeniorityLevelType | null) => void;
	onSeniorityChange: (newValue: string | null) => void;
	specialization: string;
	hardSkills: Array<FormFullHardSkillInfo>;
	seniority?: SeniorityLevelType | null;
	seniorityError?: string;
	errors?: FormikErrors<Array<FormFullHardSkillInfo>>;
	touched?: boolean[];
}
