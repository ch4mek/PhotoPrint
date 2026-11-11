import { Dispatch, SetStateAction } from 'react';

import {
	FilterSetter,
	TimesheetFilterChangeHandler,
} from '../TimeSheetControlsBar/types';

import { filterByPeriod } from '@/constants/filters';
import { formatMonthLong } from '@/helpers/formatRuDays';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { FilterOption } from './types';

export const setFilterByIdIfChanged = <T extends { id: string | number }>(
	setter: Dispatch<SetStateAction<T>>,
	nextValue: T
) => {
	setter((prev) => (prev.id === nextValue.id ? prev : nextValue));
};

export const createFilterSetter =
	(
		originalSetter: FilterSetter,
		initialValue: FilterOption
	): TimesheetFilterChangeHandler =>
	(newFilter) => {
		const isCleared = !newFilter;
		const isInitialSelection = newFilter?.id === initialValue.id;

		const nextValue =
			isCleared || isInitialSelection ? initialValue : newFilter;

		setFilterByIdIfChanged(originalSetter, nextValue);
	};

export function addWeek(date: Date) {
	const newDate = new Date(date.getTime());
	newDate.setDate(newDate.getDate() + 7);

	return newDate;
}

export function subtractWeek(date: Date) {
	const newDate = new Date(date.getTime());
	newDate.setDate(newDate.getDate() - 7);

	return newDate;
}

export function modifyMonth(date: Date, operation: 'add' | 'subtract') {
	let modifiedDate = dayjs(date);

	if (operation === 'add') {
		modifiedDate = modifiedDate.add(1, 'months');
	} else {
		modifiedDate = modifiedDate.subtract(1, 'months');
	}

	const daysInMonth = modifiedDate.daysInMonth();
	const currentMonthDay = modifiedDate.date(daysInMonth);

	return currentMonthDay.toDate();
}

export function getCurrentWeek(date = new Date()) {
	return Array(7)
		.fill(new Date(date))
		.map((day, idx) => {
			const currentDayOfWeek = day.getDay();

			return new Date(
				day.setDate(
					day.getDate() - day.getDay() + idx + (currentDayOfWeek === 0 ? -6 : 1)
				)
			);
		});
}

export function getCurrentMonth(date = new Date()) {
	let daysInMonth = dayjs(date).daysInMonth();

	const arrDays = [];

	while (daysInMonth) {
		const current = dayjs(date).date(daysInMonth);
		arrDays.push(current);
		daysInMonth -= 1;
	}

	return arrDays
		.map((item) => item.format('llll'))
		.reverse() as unknown as Dayjs[];
}

export function getCurrentMonday() {
	const today = new Date();
	const currentDayOfWeek = today.getDay();
	const difference =
		today.getDate() - currentDayOfWeek + (currentDayOfWeek === 0 ? -6 : 1);
	const day = new Date(today.setDate(difference));

	return new Date(day);
}

export const isCurrentMonth = (
	month: number,
	dateYear: number,
	locale: string
) =>
	dayjs().month() + 1 === month && dateYear === dayjs().year()
		? filterByPeriod.monthFilter
		: `${formatMonthLong(month, dateYear, locale)}, ${dateYear}`;

const isCurrentWeek = (
	currentWeek: number,
	weekDisplay: string,
	dateYear: number
) =>
	dayjs().week() === currentWeek && dateYear === dayjs().year()
		? filterByPeriod.weekFilter
		: weekDisplay;

export const selectValueByPeriod = (
	periodFilter: string,
	currentWeek: number,
	weekDisplay: string,
	month: number,
	dateYear: number,
	locale: string
) =>
	periodFilter === filterByPeriod.weekFilter
		? isCurrentWeek(currentWeek, weekDisplay, dateYear)
		: isCurrentMonth(month, dateYear, locale);
