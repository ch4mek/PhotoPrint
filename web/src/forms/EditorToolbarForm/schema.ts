import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const validationSchema = yup.object({
	url: yup.string().required(FIELD_IS_REQUIRED).url(),
});
