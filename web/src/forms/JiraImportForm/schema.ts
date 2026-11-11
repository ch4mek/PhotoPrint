import * as yup from 'yup';

import { validateDateRangeWithinMaxYears } from '../utils';

import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

import { fieldsNames } from './constants';

const { dateFrom, dateTo } = fieldsNames;

export const validationSchema = yup.object({
	dateFrom: dateType2FromDate(dateTo, undefined, undefined, true),
	dateTo: dateType2ToDate(
		dateFrom,
		undefined,
		undefined,
		true,
		validateDateRangeWithinMaxYears()
	),
	project: ddlType1Schema(),
});
