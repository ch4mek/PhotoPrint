import * as yup from 'yup';

import { getPleaseEnterAValidFieldErrorMessage } from '@/constants/errors';
import {
	checkIfNoLettersOrNumbers,
	checkToMultipleSpacesAtOnce,
} from '@/forms/utils';

export const longTextType2Schema = (fieldName: string) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);

	return yup
		.string()
		.test(
			'only-symbols-check',
			NOT_VALID_FIELD,
			(value) => !checkIfNoLettersOrNumbers(value)
		)
		.test('multiple-spaces-at-once-check', NOT_VALID_FIELD, (value) =>
			checkToMultipleSpacesAtOnce(value)
		);
};
