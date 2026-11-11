import { FormikErrors, FormikTouched } from 'formik';

import { TechnologyFormItem, TechnologyHardSkillFormItem } from '../../types';

import { AutoselectOption } from '@/types/common';
import { InternshipSeniority } from '@/types/internship';

export type TechnologyHardSkillRowProps = {
	technologyIndex: number;
	skillIndex: number;
	technology: TechnologyFormItem;
	skill: TechnologyHardSkillFormItem;
	hardSkillOptions: AutoselectOption[];
	seniorityOptions: AutoselectOption<InternshipSeniority>[];
	onFieldChange: (fieldName: string) => (value: unknown) => void;
	onRemove: () => void;
	showRemoveButton: boolean;
	hasAttemptedSubmit: boolean;
	isRemoveSpecializationButtonShown: boolean;
	skillError?: FormikErrors<TechnologyHardSkillFormItem>;
	skillTouched?: FormikTouched<TechnologyHardSkillFormItem>;
};
