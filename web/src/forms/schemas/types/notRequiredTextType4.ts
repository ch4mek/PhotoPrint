import * as yup from 'yup';

import {
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { checkIfNoLettersOrNumbers } from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

const MIN_CHARACTERS = 2;
const MAX_CHARACTERS = 250;

export const notRequiredTextType4Schema = (fieldName: string) => {
	const validityMessage = getPleaseEnterAValidFieldErrorMessage(fieldName);

	return yup
		.string()
		.nullable()
		.transform((value) => (value ? getTrimedString(value) : value))
		.test('trimmed-empty-validity', validityMessage, trimmedEmptyValidityTest)
		.test('only-symbols-check', validityMessage, (value) => {
			if (!value) {
				return true;
			}

			return !checkIfNoLettersOrNumbers(value);
		})
		.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
};
