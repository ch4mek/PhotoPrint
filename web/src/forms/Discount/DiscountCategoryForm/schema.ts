import * as yup from 'yup';

import { notRequiredTextType4Schema } from '@/forms/Discount/schema';
import { textType4Schema } from '@/forms/schemas';

export const discountCategoryValidationSchema = yup.object({
	name: textType4Schema('category name'),
	discountCategoryTags: yup.array(),
	tagName: yup.string().nullable(),
});

export const tagNameSchema = notRequiredTextType4Schema('new tag');
