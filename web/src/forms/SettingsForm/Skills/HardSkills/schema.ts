import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { textType3Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	name: textType3Schema('hard skill name'),
	hardSkillGroupId: yup.mixed().required(FIELD_IS_REQUIRED),
});
