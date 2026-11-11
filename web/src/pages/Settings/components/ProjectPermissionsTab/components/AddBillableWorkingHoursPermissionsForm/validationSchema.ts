import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const validationSchema = yup.object().shape({
	user: yup.object().nullable().required(FIELD_IS_REQUIRED),
	projects: yup.array().min(1, FIELD_IS_REQUIRED).required(FIELD_IS_REQUIRED),
});
