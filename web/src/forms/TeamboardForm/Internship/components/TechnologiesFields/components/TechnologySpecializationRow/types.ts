import { FormikErrors, FormikTouched } from 'formik';

import { SetFieldValue, TechnologyFormItem } from '../../types';

import { AutoselectOption } from '@/types/common';
import { InternshipSeniority } from '@/types/internship';

export type TechnologySpecializationRowProps = {
	technologyIndex: number;
	technology: TechnologyFormItem;
	technologies: TechnologyFormItem[];
	specializationOptions: AutoselectOption[];
	seniorityOptions: AutoselectOption<InternshipSeniority>[];
	getHardSkillOptions: (specializationId: number) => AutoselectOption[];
	onFieldChange: (fieldName: string) => (value: unknown) => void;
	setFieldValue: SetFieldValue;
	onRemoveSpecialization: () => void;
	showRemoveSpecializationButton: boolean;
	technologyError?: FormikErrors<TechnologyFormItem>;
	technologyTouched?: FormikTouched<TechnologyFormItem>;
	hasAttemptedSubmit: boolean;
};
