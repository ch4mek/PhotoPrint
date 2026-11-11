import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const validationSchema = yup.object().shape({
	location: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
});
