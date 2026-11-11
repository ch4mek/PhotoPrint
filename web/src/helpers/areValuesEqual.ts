import isEqual from 'lodash/isEqual';
import mapValues from 'lodash/mapValues';

import { FromToProps } from '@/components/Grid';
import { DateRangeFilter } from '@/components/Grid/modals/types';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { PrimitiveFilterValue } from '@/types/filter';
import { Booleanify } from '@/types/helpers';

export type Comparator<ValueType> = (a: ValueType, b: ValueType) => boolean;

export const areDatesEqual: Comparator<Date | null> = (a, b) =>
	dayjs(a).format(YYYY_MM_DD_DAYJS_DATE_FORMAT) ===
	dayjs(b).format(YYYY_MM_DD_DAYJS_DATE_FORMAT);

export const arePrimitiveValuesEqual: Comparator<
	number | string | boolean | null | undefined | symbol | bigint
> = (a, b) => a === b;

export const areArraysEqualByPrimitiveItems: Comparator<(number | string)[]> = (
	a,
	b
) => a?.length === b?.length && a?.every((item) => b.includes(item));

export const areFromToCompatibleObjectsEqual: Comparator<
	FromToProps<Date | string>
> = (a, b) => {
	if (typeof (a.from || a.to || b.from || b.to) === 'object') {
		return (
			areDatesEqual(a.from as Date, b.from as Date) &&
			areDatesEqual(a.to as Date, b.to as Date)
		);
	}

	return a?.from === b?.from && a?.to === b?.to;
};

const isFromToCompatibleFilterValue = (
	v: unknown
): v is FromToProps<Date | string> =>
	typeof v === 'object' &&
	v !== null &&
	!Array.isArray(v) &&
	('from' in v || 'to' in v);

const isPrimitiveFilterValue = (v: unknown): v is PrimitiveFilterValue =>
	v === null || (!Array.isArray(v) && typeof v !== 'object');

const areFilterFieldValuesEqual = (
	current: unknown,
	initial: unknown
): boolean => {
	if (Array.isArray(current) && Array.isArray(initial)) {
		return areArraysEqualByPrimitiveItems(current, initial);
	}

	if (
		isFromToCompatibleFilterValue(current) &&
		isFromToCompatibleFilterValue(initial)
	) {
		return areFromToCompatibleObjectsEqual(current, initial);
	}

	if (isPrimitiveFilterValue(current) && isPrimitiveFilterValue(initial)) {
		return arePrimitiveValuesEqual(current, initial);
	}

	return false;
};

export const areFiltersEqualByKeys = <T extends object>(
	current: T,
	initial: T,
	keys: ReadonlyArray<keyof T>
): boolean =>
	keys.every((key) => areFilterFieldValuesEqual(current[key], initial[key]));

export const areDateRangesEqual: Comparator<DateRangeFilter> = (a, b) =>
	a.includeNull === b.includeNull &&
	areDatesEqual(a.fromDate, b.fromDate) &&
	areDatesEqual(a.toDate, b.toDate);

export const areObjectsFieldEqual = <T, K extends keyof T>(
	a: T,
	b: T,
	key: K
): boolean => a[key] === b[key];

export const areObjectsShallowlyEqual = <T extends Record<string, any>>(
	obj1: T,
	obj2: T
): Booleanify<T> => mapValues(obj1, (value, key) => isEqual(value, obj2[key]));

export const areCaseInsensitiveStringsEqual = (
	value1?: string | null,
	value2?: string | null
): boolean => {
	if (!value1 || !value2) {
		return false;
	}

	return value1.toLowerCase() === value2.toLowerCase();
};
