import * as yup from 'yup';

import { MAX_NUMBER_TYPE_1_DIGITS } from '../constants';

import {
	getMaxLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	getDigitsRegExp,
	getDigitsWithMaxLengthRegExp,
} from '@/constants/regexp';

export const numberType1Schema = (
	fieldName: string,
	maxChars: number = MAX_NUMBER_TYPE_1_DIGITS,
	zeroNotAllowedMessage: string = getPleaseEnterAValidFieldErrorMessage(
		fieldName
	),
	customErrorMessage?: string,
	allowZero: boolean = false
) =>
	yup
		.string()
		.matches(
			getDigitsRegExp(),
			customErrorMessage ?? getPleaseEnterAValidFieldErrorMessage(fieldName)
		)
		.matches(
			getDigitsWithMaxLengthRegExp(maxChars),
			getMaxLengthErrorMessage(maxChars)
		)
		.test('is-not-zero', zeroNotAllowedMessage, (value) => {
			if (!value) {
				return true;
			}

			const numericValue = Number(value);

			if (allowZero) {
				return numericValue >= 0;
			}

			return numericValue > 0;
		});
