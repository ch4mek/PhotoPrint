import * as yup from 'yup';

import { DATE_FORMAT } from '@/constants/date';
import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
} from '@/constants/errors';
import { DATE_REG_EXP } from '@/constants/regexp';
import {
	DATE_TYPE_1_MAX_CHARACTERS,
	MAX_CHARACTERS,
} from '@/forms/schemas/constants';
import { INVALID_DATE_FORMAT_MESSAGE } from '@/forms/TeamboardForm/EmploymentHistory/constants';
import dayjs from '@/lib/dayjs';

export const dateType1Schema = (isMandatory = true) => {
	const schema = yup
		.string()
		.nullable()
		.transform((value) => {
			if (value) {
				const dateValue = dayjs.isDayjs(value) ? value : dayjs(new Date(value));

				if (!dateValue.isValid()) {
					return isMandatory ? null : DATE_FORMAT;
				}

				return dateValue.format(DATE_FORMAT);
			}

			return value;
		})
		.required(FIELD_IS_REQUIRED)
		.max(DATE_TYPE_1_MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));

	if (!isMandatory) {
		return schema
			.notRequired()
			.matches(DATE_REG_EXP, INVALID_DATE_FORMAT_MESSAGE);
	}

	return schema;
};
