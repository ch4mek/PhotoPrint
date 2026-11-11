import * as yup from 'yup';

import {
	NOT_VALID_PHONE_FORMAT,
	NOT_VALID_PHONE_WITH_PLUS_FORMAT,
	getMaxLengthErrorMessage,
} from '@/constants/errors';
import { PHONE_REG_EXP } from '@/constants/regexp';
import {
	checkEmptyField,
	checkIfNoLettersOrNumbers,
	checkToAllHyphens,
	checkToAllHyphensAndSpaces,
	checkToAllSpaces,
} from '@/forms/utils';

export const phoneNumberType2Schema = () => {
	const MAX_CHARACTERS = 15;

	return yup
		.string()
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
		.test('all-spaces-check', NOT_VALID_PHONE_FORMAT, (value) =>
			checkToAllSpaces(value)
		)
		.test('all-hyphens-check', NOT_VALID_PHONE_FORMAT, (value) =>
			checkToAllHyphens(value)
		)
		.test('all-hyphens-and-spaces-check', NOT_VALID_PHONE_FORMAT, (value) =>
			checkToAllHyphensAndSpaces(value)
		)
		.nullable()
		.test('only-symbols-check', NOT_VALID_PHONE_FORMAT, (value) => {
			if (value?.length) {
				return !checkIfNoLettersOrNumbers(value);
			}

			return !checkEmptyField(value);
		})
		.matches(PHONE_REG_EXP, NOT_VALID_PHONE_WITH_PLUS_FORMAT);
};
