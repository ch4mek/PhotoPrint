import * as yup from 'yup';

import { specializationSchema } from '../schemas/hardSkillsSchemas';

import { dateType1Schema } from '@/forms/schemas';

export const validationSchema = yup.object().shape({
	date: dateType1Schema(),
	specialization: specializationSchema,
});
