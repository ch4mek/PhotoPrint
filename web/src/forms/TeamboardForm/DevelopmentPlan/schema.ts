import * as yup from 'yup';

import { longTextType1Schema } from '@/forms/schemas';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';

const startDate = 'startDate';
const endDate = 'endDate';
const deadlineDate = 'deadlineDate';
const summary = 'summary';
const notes = 'notes';

export const developmentPlanValidationSchema = yup.object({
	[startDate]: dateType2FromDate([endDate, deadlineDate]),
	[deadlineDate]: dateType2ToDate(startDate, true),
	[endDate]: dateType2ToDate(startDate, true),
	[summary]: longTextType1Schema(summary, true),
	[notes]: longTextType1Schema(notes),
});
