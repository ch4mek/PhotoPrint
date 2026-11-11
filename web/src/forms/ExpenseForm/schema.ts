import * as yup from 'yup';

import { textType4Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	expense: textType4Schema('expense name'),
});
