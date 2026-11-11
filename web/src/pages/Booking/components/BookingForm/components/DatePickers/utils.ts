import { Timezone } from '@/components/Timeline/types';
import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { INVALID_DATE_RANGE } from '@/constants/notifications';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { DatePickerError } from './constants';

export const getDatePickerErrorMessage = (error: string | null) => {
	switch (error) {
		case DatePickerError.InvalidDate:
			return FIELD_IS_REQUIRED;
		case DatePickerError.PastDate:
		case DatePickerError.MinDate:
		case DatePickerError.MaxDate:
			return INVALID_DATE_RANGE;
		default:
			return undefined;
	}
};

export const getParseInputDateToDate =
	(timezone: Timezone) =>
	(date: string[]): Dayjs | null => {
		if (!date.length) {
			return null;
		}

		const [day, month, year] = date;

		let inputDate = dayjs().tz(timezone);

		inputDate = inputDate.set({
			date: +day,
			month: +month - 1,
			year: +year,
		});

		if (!inputDate.isValid()) {
			return null;
		}

		return inputDate;
	};
