import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

const specializationHardSkillSchema = yup.object().shape({
	info: ddlType1Schema(),
	seniority: ddlType1Schema(false),
});

export const specializationSchema = yup
	.object()
	.shape({
		hardSkills: yup.array().of(specializationHardSkillSchema),
		seniority: ddlType1Schema(),
	})
	.nullable()
	.required(FIELD_IS_REQUIRED);
