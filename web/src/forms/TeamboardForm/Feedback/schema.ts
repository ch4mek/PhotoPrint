import * as yup from 'yup';

import {
	dateType1Schema,
	longTextType1Schema,
	textType1Dot1Schema,
} from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

export const feedbackValidationSchema = yup.object({
	date: dateType1Schema(),
	type: ddlType1Schema(),
	project: ddlType1Schema(),
	feedback: longTextType1Schema('feedback', true),
	actions: longTextType1Schema('our actions'),
	conversation: ddlType1Schema(false),
	reviewer: textType1Dot1Schema('reviewer', false),
});
