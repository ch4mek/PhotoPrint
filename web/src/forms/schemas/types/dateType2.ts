import * as yup from 'yup';

import { DATE_RANGE_IS_INVALID, FIELD_IS_REQUIRED } from '@/constants/errors';
import { INVALID_DATE_FORMAT_MESSAGE } from '@/forms/TeamboardForm/EmploymentHistory/constants';
import { DateRangeValidator } from '@/forms/types';
import { isDateMoreThanCurrent } from '@/helpers/date';
import { isDateRangeValid } from '@/helpers/isDateRangeValid';
import dayjs from '@/lib/dayjs';

const checkDateRange = (
	toDatePropName: string | string[],
	fromDate: Date,
	context: yup.TestContext,
	allowEqualToFromDate: boolean
) => {
	const toDateNames =
		typeof toDatePropName === 'string' ? [toDatePropName] : toDatePropName;

	return toDateNames.every((toDateField) => {
		const toDate = context.parent[toDateField];

		return dayjs(toDate).isValid()
			? isDateRangeValid(fromDate, toDate, allowEqualToFromDate)
			: true;
	});
};

export const dateType2FromDate = (
	toDatePropName: string | string[],
	isOptional = false,
	onlyFutureTime = false,
	allowEqualToFromDate = false
) => {
	const fromDateSchema = yup
		.date()
		.typeError(isOptional ? INVALID_DATE_FORMAT_MESSAGE : FIELD_IS_REQUIRED)
		.transform((value) => (value && dayjs(value).isValid() ? value : null))
		.nullable()
		.required(FIELD_IS_REQUIRED)
		.test('is-from-date-valid', DATE_RANGE_IS_INVALID, (fromDate, context) => {
			if (!fromDate || !dayjs(fromDate).isValid()) {
				return true;
			}

			const isFutureDate = onlyFutureTime
				? isDateMoreThanCurrent(fromDate)
				: true;

			if (toDatePropName) {
				return (
					isFutureDate &&
					checkDateRange(
						toDatePropName,
						fromDate,
						context,
						allowEqualToFromDate
					)
				);
			}

			return isFutureDate;
		});

	if (isOptional) {
		return fromDateSchema.notRequired();
	}

	return fromDateSchema;
};

export const dateType2ToDate = (
	fromDatePropName: string,
	isOptional = false,
	onlyFutureTime = false,
	allowEqualToFromDate = false,
	validateDateRange?: DateRangeValidator
) => {
	let toDateSchema = yup
		.date()
		.nullable()
		.test('is-end-date-valid', DATE_RANGE_IS_INVALID, (toDate, context) => {
			const fromDate = context.parent[fromDatePropName];

			if (
				!toDate ||
				!dayjs(toDate).isValid() ||
				!fromDate ||
				!dayjs(fromDate).isValid()
			) {
				return true;
			}

			if (onlyFutureTime) {
				return (
					isDateMoreThanCurrent(toDate) &&
					isDateRangeValid(fromDate, toDate, allowEqualToFromDate)
				);
			}

			return isDateRangeValid(fromDate, toDate, allowEqualToFromDate);
		})
		.test('is-after-fromDate', DATE_RANGE_IS_INVALID, (toDate, context) => {
			const fromDate = context.parent[fromDatePropName];

			if (
				!toDate ||
				!dayjs(toDate).isValid() ||
				!fromDate ||
				!dayjs(fromDate).isValid()
			) {
				return true;
			}

			return dayjs(toDate).isSameOrAfter(fromDate, 'date');
		});

	if (validateDateRange) {
		toDateSchema = toDateSchema.when(
			fromDatePropName,
			(fromDateValue, schema) =>
				schema.test({
					name: 'custom-date-range',
					message: DATE_RANGE_IS_INVALID,
					test(toDateValue: Date | null) {
						if (
							!fromDateValue ||
							!toDateValue ||
							!dayjs(fromDateValue).isValid() ||
							!dayjs(toDateValue).isValid()
						) {
							return true;
						}

						return validateDateRange(fromDateValue, toDateValue);
					},
				})
		);
	}

	if (!isOptional) {
		toDateSchema = toDateSchema.required(FIELD_IS_REQUIRED);
	}

	return toDateSchema.typeError(
		isOptional ? INVALID_DATE_FORMAT_MESSAGE : FIELD_IS_REQUIRED
	);
};
