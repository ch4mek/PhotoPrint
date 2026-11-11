import * as yup from 'yup';

import { textType4Schema } from '../schemas';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const validationSchema = yup.object({
	name: textType4Schema('project name'),
	client: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	manager: yup.mixed().nullable(),
});
