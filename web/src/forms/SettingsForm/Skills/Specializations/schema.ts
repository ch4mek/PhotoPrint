import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { textType3Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	name: textType3Schema('specialization name'),
	departments: yup
		.array()
		.min(1, FIELD_IS_REQUIRED)
		.required(FIELD_IS_REQUIRED),
	hardSkills: yup.array(),
});
