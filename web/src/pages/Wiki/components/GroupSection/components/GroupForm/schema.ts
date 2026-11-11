import * as yup from 'yup';

import { textType4Schema } from '@/forms/schemas';
import { SegmentTypes } from '@/types/wiki';

export const getValidationSchema = (fieldName: SegmentTypes) =>
	yup.object({
		name: textType4Schema(fieldName.toLowerCase()),
	});
