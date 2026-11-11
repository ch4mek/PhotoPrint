import * as yup from 'yup';

import { textType3Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	name: textType3Schema('soft skill name'),
});
