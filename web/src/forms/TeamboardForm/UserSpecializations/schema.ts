import * as yup from 'yup';

import { specializationSchema } from '../schemas/hardSkillsSchemas';

import { dateType1Schema } from '@/forms/schemas';

export const validationSchema = yup.object().shape({
	snapshotDate: dateType1Schema(),
	userSpecializations: yup.array().of(specializationSchema),
});
