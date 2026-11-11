import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { CURRENCY_REG_EXP } from '@/constants/regexp';
import { calculateLengthWithoutCommas } from '@/forms/utils';

export const currencyType1Schema = (fieldName: string) => {
	const MAX_CHARACTERS = 13;

	return yup
		.string()
		.required(FIELD_IS_REQUIRED)
		.matches(CURRENCY_REG_EXP, getPleaseEnterAValidFieldErrorMessage(fieldName))
		.test(
			'max-length',
			getMaxLengthErrorMessage(MAX_CHARACTERS),
			(value) => calculateLengthWithoutCommas(value) <= MAX_CHARACTERS
		);
};
