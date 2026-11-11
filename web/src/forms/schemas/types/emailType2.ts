import * as yup from 'yup';

import {
	NOT_VALID_EMAIL_FORMAT,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
} from '@/constants/errors';
import { EMAIL_REG_EXP_DOMAIN } from '@/constants/regexp';

export const emailType2Schema = () => {
	const MIN_CHARACTERS = 5;
	const MAX_CHARACTERS = 550;

	return yup
		.string()
		.nullable()
		.transform((value?: string | null) => {
			if (typeof value !== 'string') {
				return value;
			}

			const trimmedValue = value.trim();

			return trimmedValue === '' ? null : trimmedValue;
		})
		.matches(EMAIL_REG_EXP_DOMAIN, NOT_VALID_EMAIL_FORMAT)
		.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
};
