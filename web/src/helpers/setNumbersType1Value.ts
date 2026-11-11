import { FormikErrors } from 'formik';

import { ONLY_DIGITS_REG_EXP } from '@/constants/regexp';
import { MAX_NUMBER_TYPE_1_DIGITS } from '@/forms/schemas/constants';

export type SetFieldValue<T> = (
	field: string,
	value: unknown,
	shouldValidate?: boolean
) => Promise<void | FormikErrors<T>>;

const getFirstDigitsRegExp = (max: number) => new RegExp(`^\\d{1,${max}}`);

export const setNumbersType1Value =
	<T>(
		setFieldValue: SetFieldValue<T>,
		maxChars: number = MAX_NUMBER_TYPE_1_DIGITS
	) =>
	(field: string, value: unknown, shouldValidate?: boolean) =>
		setFieldValue(
			field,
			String(value).match(
				maxChars ? getFirstDigitsRegExp(maxChars) : ONLY_DIGITS_REG_EXP
			)?.[0] ?? '',
			shouldValidate
		);
