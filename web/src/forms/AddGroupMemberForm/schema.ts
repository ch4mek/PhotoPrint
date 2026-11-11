import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const addGroupMemberValidationSchema = yup.object({
	user: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
});
