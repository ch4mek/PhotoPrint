import * as yup from 'yup';

import { dateType1Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

const skillsSchema = yup
	.object()
	.shape({
		seniority: ddlType1Schema(),
	})
	.nullable();

export const skillsValidationSchema = yup.object().shape({
	date: dateType1Schema(),
	skills: yup.array().of(skillsSchema),
});
