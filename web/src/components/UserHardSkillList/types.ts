import { FormikErrors } from 'formik';

import { SeniorityLevelType } from '@/types/configuration';
import { FormFullHardSkillInfo } from '@/types/teamboard';

export interface UserHardSkillListProps {
	hardSkills: Array<FormFullHardSkillInfo>;
	onSeniorityChange: (
		hardSkillId: number
	) => (newSeniority: SeniorityLevelType | null) => void;
	errors?: FormikErrors<Array<FormFullHardSkillInfo>>;
	touched?: boolean[];
	senioritiesDisabled?: boolean;
}
