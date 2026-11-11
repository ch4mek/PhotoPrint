import * as yup from 'yup';

import { dateType1Schema } from '@/forms/schemas';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';

import { EmploymentHistoryFields } from './constants';

export const validationSchema = yup.object().shape({
	professionalActivityStartDate: dateType1Schema(false),
	[EmploymentHistoryFields.HIRE_DATE]: dateType2FromDate(
		EmploymentHistoryFields.FIRE_DATE
	),
	[EmploymentHistoryFields.FIRE_DATE]: dateType2ToDate(
		EmploymentHistoryFields.HIRE_DATE,
		true
	),
	[EmploymentHistoryFields.NDA_START_DATE]: dateType1Schema(false),
	[EmploymentHistoryFields.NDA_END_DATE]: dateType1Schema(false),
});
