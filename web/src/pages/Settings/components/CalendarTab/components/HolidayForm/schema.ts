import * as yup from 'yup';

import { DATE_RANGE_IS_INVALID, FIELD_IS_REQUIRED } from '@/constants/errors';
import { dateType1Schema, textType4Schema } from '@/forms/schemas';

import { HOLIDAY_FORM_FIELDS } from './constants';

export const getValidationSchema = (isHolidayTypeMode: boolean) =>
	yup.object({
		countryId: yup.mixed().required(FIELD_IS_REQUIRED),
		date: dateType1Schema().test(
			'dates-not-equal',
			DATE_RANGE_IS_INVALID,
			(value, context) => {
				const { newDate } = context.parent;

				return !(value && newDate && value === newDate);
			}
		),
		newDate: isHolidayTypeMode
			? yup.mixed().notRequired()
			: dateType1Schema().test(
					'dates-not-equal',
					DATE_RANGE_IS_INVALID,
					(value, context) => {
						const { date } = context.parent;

						if (isHolidayTypeMode) {
							return true;
						}

						return !(value && date && value === date);
					}
			  ),
		name: isHolidayTypeMode
			? textType4Schema(HOLIDAY_FORM_FIELDS.name.name).required(
					FIELD_IS_REQUIRED
			  )
			: yup.string().nullable().notRequired(),
	});
