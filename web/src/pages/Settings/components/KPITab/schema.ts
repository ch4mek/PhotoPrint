import * as yup from 'yup';

import { FIELD_IS_REQUIRED, INVALID_PERCENT_FORMAT } from '@/constants/errors';
import { numberType1Schema } from '@/forms/schemas';

const colorPercentSchema = (fieldName: string) =>
	yup
		.string()
		.transform(({ rightIntervalBorder }) => rightIntervalBorder)
		.concat(numberType1Schema(fieldName, undefined, INVALID_PERCENT_FORMAT))
		.required(FIELD_IS_REQUIRED);

export const validationSchema = yup.object().shape({
	minColor: colorPercentSchema('min color'),
	middleColor: colorPercentSchema('middle color'),
});
