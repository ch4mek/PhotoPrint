import * as yup from 'yup';

import { ddlType1Schema } from '@/forms/schemas';

import { TECHNOLOGIES_FIELDS_DATA } from './constants';

const hardSkillItemSchema = yup.object({
	rowId: yup.string().required(),
	hardSkill: ddlType1Schema(),
	seniority: ddlType1Schema(false),
});

const technologyItemSchema = yup.object({
	rowId: yup.string().required(),
	id: yup.number().nullable(),
	specialization: ddlType1Schema(false),
	seniority: yup.mixed().when(TECHNOLOGIES_FIELDS_DATA.specialization.name, {
		is: (value: unknown) => value != null,
		then: () => ddlType1Schema(),
		otherwise: () => ddlType1Schema(false),
	}),
	hardSkills: yup
		.array()
		.of(hardSkillItemSchema)
		.when(TECHNOLOGIES_FIELDS_DATA.specialization.name, {
			is: (value: unknown) => value != null,
			then: (schema) => schema.min(1),
			otherwise: (schema) => schema.optional(),
		}),
});

export const technologiesValidationSchema = yup.object({
	technologies: yup.array().of(technologyItemSchema),
});
