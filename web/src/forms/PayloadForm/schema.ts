import * as yup from 'yup';

import { longTextType1Schema } from '@/forms/schemas';

export const payloadValidationSchema = yup.object({
	comment: longTextType1Schema('comment'),
	plan: longTextType1Schema('plan'),
});
