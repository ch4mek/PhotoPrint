import * as yup from 'yup';

import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';

import { DevelopmentPlanApprovingFormField } from './types';

export const developmentPlanApprovingValidationSchema = yup.object({
	[DevelopmentPlanApprovingFormField.StartDate]: dateType2FromDate(
		DevelopmentPlanApprovingFormField.ConfirmationDate
	),
	[DevelopmentPlanApprovingFormField.ConfirmationDate]: dateType2ToDate(
		DevelopmentPlanApprovingFormField.StartDate
	),
});
