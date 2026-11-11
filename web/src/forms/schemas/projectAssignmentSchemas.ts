import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	VALUE_MUST_BE_AT_LEAST_0,
	VALUE_MUST_BE_AT_MOST_1,
} from '@/constants/errors';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

const startDateKey = 'startDate';
const endDateKey = 'endDate';

const commonValidationSchema = yup.object({
	[startDateKey]: dateType2FromDate(endDateKey, undefined, undefined, true),
	[endDateKey]: dateType2ToDate(startDateKey, true, undefined, true),
	projectSeniority: ddlType1Schema(),
	fte: yup
		.number()
		.nullable(true)
		.required(FIELD_IS_REQUIRED)
		.min(0, VALUE_MUST_BE_AT_LEAST_0)
		.max(1, VALUE_MUST_BE_AT_MOST_1),
	billability: yup.boolean(),
});

export const projectAssignmentCreatingSchema = commonValidationSchema.shape({
	user: ddlType1Schema(),
	project: ddlType1Schema(),
});

export const userProjectAssignmentEditingSchema = commonValidationSchema.shape({
	project: ddlType1Schema(),
});

export const projectAssignmentEditingSchema = commonValidationSchema.shape({
	user: ddlType1Schema(),
});
