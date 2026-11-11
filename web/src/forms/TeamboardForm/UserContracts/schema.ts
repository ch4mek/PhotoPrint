import * as yup from 'yup';

import { DATE_SEPARATOR } from '@/constants/date';
import { DATE_RANGE_IS_INVALID } from '@/constants/errors';
import { dateType1Schema, longTextType1Schema } from '@/forms/schemas';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';
import dayjs from '@/lib/dayjs';

const startDatePropName = 'startDate';
const endDatePropName = 'endDate';
const fireDatePropName = 'fireDate';
const fireReasonPropName = 'fire reason';
const fireDescriptionPropName = 'fire description';

export const validationSchema = yup.object().shape({
	[startDatePropName]: dateType2FromDate(endDatePropName),
	[endDatePropName]: dateType2ToDate(startDatePropName),
	[fireDatePropName]: dateType1Schema(false).test(
		'is-in-range',
		'',
		(value, testContext) => {
			const [startDate, endDate] = [
				testContext.parent[startDatePropName],
				testContext.parent[endDatePropName],
			];

			if (!value) {
				return true;
			}

			const fireDateDayjs = dayjs(
				value.split(DATE_SEPARATOR).reverse().join(DATE_SEPARATOR)
			);

			const startDateDayjs = startDate ? dayjs(startDate) : null;
			const endDateDayjs = endDate ? dayjs(endDate) : null;

			const errors = testContext.createError({
				message: () =>
					[startDatePropName, endDatePropName, fireDatePropName].map(
						(propName) =>
							testContext.createError({
								message: DATE_RANGE_IS_INVALID,
								path: propName,
							})
					),
			});

			if (startDateDayjs && startDateDayjs >= fireDateDayjs) {
				return errors;
			}

			if (endDateDayjs && endDateDayjs < fireDateDayjs) {
				return errors;
			}

			return true;
		}
	),
	fireType: yup.mixed().optional(),
	fireReason: longTextType1Schema(fireReasonPropName),
	fireDescription: longTextType1Schema(fireDescriptionPropName),
});
