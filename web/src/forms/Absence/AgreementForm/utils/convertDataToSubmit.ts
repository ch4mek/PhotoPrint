import { InitialFormValues } from '../types';

import { Agreement, AgreementItem } from '@/types/absences';

export const convertDataToSubmit = (
	values: InitialFormValues,
	inquiryTemplateId: number
): Agreement => ({
	...values,
	inquiryTemplateId,
});

export const convertUpdatedDataToSubmit = (
	values: InitialFormValues,
	id: number | string
): AgreementItem => ({
	...values,
	id,
});
