import * as yup from 'yup';

import { FIELD_IS_REQUIRED, getEnterAValidMessage } from '@/constants/errors';
import { ONLY_POSITIVE_DIGITS_REG_EXP } from '@/constants/regexp';

export const numberType2Schema = (fieldName: string) =>
	yup
		.string()
		.required(FIELD_IS_REQUIRED)
		.matches(ONLY_POSITIVE_DIGITS_REG_EXP, getEnterAValidMessage(fieldName));
