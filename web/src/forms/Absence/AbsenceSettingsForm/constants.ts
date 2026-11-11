import * as yup from 'yup';

import { longTextType1Schema } from '@/forms/schemas';

import { AbsenceSettingsFormValues } from './types';

export const POPUP_TITLE = 'Absence setting';
export const FORM_NAME = 'Absence setting form';

export const COUNTER_LABEL = 'Adjust absence amount by';

const ABSENCE_AMOUNT_INITIAL_VALUE = 0;
export const ABSENCE_AMOUNT_MIN_VALUE = -99.99;
export const ABSENCE_AMOUNT_MAX_VALUE = 99.99;
export const ABSENCE_PRECISION = 2;

export const validationSchema = yup.object().shape({
	amount: yup
		.number()
		.test('is-not-zero', 'Days count cannot be 0', (value) => {
			if (value === null || value === undefined) {
				return true;
			}

			return value !== 0;
		}),
	comment: longTextType1Schema('comment'),
});

export const INITIAL_VALUES: AbsenceSettingsFormValues = {
	amount: ABSENCE_AMOUNT_INITIAL_VALUE,
	comment: '',
};
