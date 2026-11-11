import * as yup from 'yup';

import { ONLY_POSITIVE_DIGITS_AND_ZERO_REG_EXP } from '@/constants/regexp';

export const numberType3Schema = () =>
	yup.string().required().matches(ONLY_POSITIVE_DIGITS_AND_ZERO_REG_EXP);
