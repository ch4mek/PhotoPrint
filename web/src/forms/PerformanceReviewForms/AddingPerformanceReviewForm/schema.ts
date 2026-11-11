import * as yup from 'yup';

import { DATE_FORMAT, MIN_FULL_DATE } from '@/constants/date';
import {
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { dateType1Schema } from '@/forms/schemas';
import dayjs, { Dayjs } from '@/lib/dayjs';

const dateRangeValidationSchema = (
	fieldName: string,
	isMandatory = true,
	minDate: Dayjs = MIN_FULL_DATE
) => {
	const dateFieldErrorMessage =
		getPleaseEnterAValidFieldErrorMessage(fieldName);

	const schema = dateType1Schema(isMandatory).test(
		'is-in-range',
		dateFieldErrorMessage,
		(value) => {
			const date = dayjs(value, DATE_FORMAT);

			return date.isValid() && date.isSameOrAfter(minDate);
		}
	);

	return schema;
};

export const validationSchema = yup.object({
	user: yup.mixed().required(FIELD_IS_REQUIRED),
	date: dateRangeValidationSchema('date'),
});
