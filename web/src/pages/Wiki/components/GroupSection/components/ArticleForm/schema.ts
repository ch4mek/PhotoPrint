import * as yup from 'yup';

import { textType4Schema } from '@/forms/schemas';

export const validationSchema = yup.object({
	name: textType4Schema('article name'),
	keywords: textType4Schema('keywords'),
});
