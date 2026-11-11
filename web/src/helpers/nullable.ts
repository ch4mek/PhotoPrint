import dayjs, { Dayjs } from '@/lib/dayjs';

type Emptyish = null | undefined | '';

export const isNotEmptyish = <T>(value: T | Emptyish): value is T =>
	value != null && value !== '';

export const toNullableDayjs = (value: Dayjs | string | Date | Emptyish) =>
	isNotEmptyish(value) ? dayjs(value) : null;

export const toNullableNumber = (value: string | number | Emptyish) =>
	isNotEmptyish(value) ? Number(value) : null;

export const toNullableString = (value: string | Emptyish) =>
	isNotEmptyish(value) ? value.trim() || null : null;
