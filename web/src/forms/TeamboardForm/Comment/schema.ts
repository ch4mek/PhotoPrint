import * as yup from 'yup';

import { longTextType1Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	comments: longTextType1Schema('comment'),
});
