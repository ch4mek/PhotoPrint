import * as yup from 'yup';

import {
	getMaxLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { TIME_REG_EXP } from '@/constants/regexp';

export const timeSchema = (fieldName: string) => {
	const MAX_CHARACTERS = 5;

	return yup
		.string()
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
		.matches(TIME_REG_EXP, getPleaseEnterAValidFieldErrorMessage(fieldName));
};
