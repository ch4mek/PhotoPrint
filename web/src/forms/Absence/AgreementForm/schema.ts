import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	INVALID_DAYS_ACCURAL_FORMATE,
} from '@/constants/errors';
import { numberType1Schema, textType4Schema } from '@/forms/schemas';

import { agreementsFieldData } from './constants';

const { name, daysPerContractYear } = agreementsFieldData;

export const validationSchema = yup.object({
	name: textType4Schema(name.label.toLowerCase()),
	daysPerContractYear: numberType1Schema(
		daysPerContractYear.label.toLowerCase(),
		undefined,
		INVALID_DAYS_ACCURAL_FORMATE,
		INVALID_DAYS_ACCURAL_FORMATE
	)
		.nullable()
		.required(FIELD_IS_REQUIRED),
});
