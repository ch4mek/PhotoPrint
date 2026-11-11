import * as yup from 'yup';

import {
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { checkIfNoLettersOrNumbers } from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';

export const taskNameValidationSchema = () => {
	const MIN_CHARACTERS = 2;
	const MAX_CHARACTERS = 250;

	return yup
		.string()
		.nullable(true)
		.transform((value) => (value ? getTrimedString(value) : value))
		.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
		.test(
			'only-symbols-check',
			getPleaseEnterAValidFieldErrorMessage('task name'),
			(value) => !checkIfNoLettersOrNumbers(value)
		);
};
