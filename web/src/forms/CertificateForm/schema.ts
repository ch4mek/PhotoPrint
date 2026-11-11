import * as yup from 'yup';

import { dateType1Schema, textType1Schema } from '../schemas';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const certificateValidationSchema = yup.object().shape({
	firstName: textType1Schema('first name'),
	lastName: textType1Schema('last name'),
	date: dateType1Schema(),
});

export const certificateWithStackValidationSchema =
	certificateValidationSchema.concat(
		yup.object().shape({
			stack: yup.mixed().required(FIELD_IS_REQUIRED),
		})
	);
