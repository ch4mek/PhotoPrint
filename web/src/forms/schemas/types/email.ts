import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	NOT_VALID_EMAIL_DOMAIN,
	NOT_VALID_EMAIL_FORMAT,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
} from '@/constants/errors';
import { EMAIL_REG_EXP, MODSEN_DOMAIN_REG_EXP } from '@/constants/regexp';

export const emailSchema = () => {
	const MIN_CHARACTERS = 25;
	const MAX_CHARACTERS = 550;

	return yup
		.string()
		.transform((value?: string | null) =>
			typeof value === 'string' ? value.trim() : value
		)
		.required(FIELD_IS_REQUIRED)
		.matches(EMAIL_REG_EXP, NOT_VALID_EMAIL_FORMAT)
		.matches(MODSEN_DOMAIN_REG_EXP, NOT_VALID_EMAIL_DOMAIN)
		.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
};
