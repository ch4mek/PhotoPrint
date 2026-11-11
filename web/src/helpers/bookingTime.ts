import { Timezone } from '@/components/Timeline/types';
import {
	MEETING_ROOMS_TAB_ID,
	NOT_WORKING_DAY_END_HOUR,
	NOT_WORKING_DAY_START_HOUR,
	RESPONSE_TIME_FORMAT,
	TIME_SELECTOR_STEP_IN_MINUTES,
	WORKING_DAY_END_HOUR,
	WORKING_DAY_START_HOUR,
} from '@/constants/booking';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { SelectedTime } from '@/context/types';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { AutoselectOption } from '@/types/common';

import { getFormattedTime } from './bookingFormatters';
import { parseDayjsToUtcString, parseStringToDayjs } from './bookingParsers';

const MINUTES_IN_HOUR = 60;
const MS_IN_SEC = 1000;
const HOURS_IN_DAY = 24;
const AVAILABLE_PERIOD_TO_BOOK_IN_MONTH = 3;

export const getDayStartHour = (isWorkingHours: boolean) =>
	isWorkingHours ? WORKING_DAY_START_HOUR : NOT_WORKING_DAY_START_HOUR;
export const getDayEndHour = (isWorkingHours: boolean) =>
	isWorkingHours ? WORKING_DAY_END_HOUR : NOT_WORKING_DAY_END_HOUR;

export function getNearestQuarterHourTime(
	date = dayjs(),
	stepCountOffset = 0,
	skipIfMultiple = false
) {
	const dateMinutes = date.minute();
	const dateHours = date.hour();

	const hasRest = dateMinutes % TIME_SELECTOR_STEP_IN_MINUTES;
	const extraMinutes = !hasRest && skipIfMultiple ? 1 : 0;

	const nearestMinutes =
		Math.ceil((dateMinutes + extraMinutes) / TIME_SELECTOR_STEP_IN_MINUTES) *
			TIME_SELECTOR_STEP_IN_MINUTES +
		TIME_SELECTOR_STEP_IN_MINUTES * stepCountOffset;

	const resultMinutes = nearestMinutes % MINUTES_IN_HOUR;
	const resultHours =
		(dateHours + Math.floor(nearestMinutes / MINUTES_IN_HOUR)) % HOURS_IN_DAY;

	return getFormattedTime(resultHours, resultMinutes);
}

const generateTimeOptions = (startHour: number, endHour: number) => {
	const options: AutoselectOption<string>[] = [];
	const possibleMinutes = [];

	for (let i = 0; i < MINUTES_IN_HOUR / TIME_SELECTOR_STEP_IN_MINUTES; i += 1) {
		possibleMinutes.push(i * TIME_SELECTOR_STEP_IN_MINUTES);
	}

	for (let i = startHour; i <= endHour; i += 1) {
		possibleMinutes.forEach((minute) => {
			const formattedTime = getFormattedTime(i, minute);

			options.push({
				id: formattedTime,
				name: formattedTime,
			});
		});
	}

	return options;
};

export const getTimeRange = (
	fromTime: Dayjs | null | undefined,
	toTime: Dayjs | null | undefined,
	timezone?: Timezone
): [Dayjs, Dayjs] => {
	let parsedFromTime =
		fromTime || parseStringToDayjs(getNearestQuarterHourTime());
	let parsedToTime = toTime || parseStringToDayjs(getNearestQuarterHourTime());

	if (timezone) {
		parsedFromTime = parsedFromTime.tz(timezone, true);
		parsedToTime = parsedToTime.tz(timezone, true);
	}

	return [
		parsedFromTime.second(0).millisecond(0),
		parsedToTime.second(0).millisecond(0),
	];
};

export const calculateTimeDifferenceInMinutes = (
	timeFrom: Dayjs | null | undefined,
	timeTo: Dayjs | null | undefined,
	timezone?: Timezone
) => {
	if (!timeFrom || !timeTo) {
		return 0;
	}

	const [dateFrom, dateTo] = getTimeRange(timeFrom, timeTo, timezone);

	const differenceMs = dateTo.toDate().getTime() - dateFrom.toDate().getTime();

	const minutes = Math.floor(differenceMs / (MS_IN_SEC * MINUTES_IN_HOUR));

	return minutes;
};

export const generateToTimeOptions = (isWorkingHours: boolean) => {
	const startHour = getDayStartHour(isWorkingHours);
	const endHour = getDayEndHour(isWorkingHours);

	const options = generateTimeOptions(startHour, endHour);

	options.shift();

	const lastOptionName = getFormattedTime((endHour + 1) % HOURS_IN_DAY, 0);

	options.push({ id: lastOptionName, name: lastOptionName });

	return options;
};

export const generateFromTimeOptions = (isWorkingHours: boolean) => {
	const startHour = getDayStartHour(isWorkingHours);
	const endHour = getDayEndHour(isWorkingHours);

	const options = generateTimeOptions(startHour, endHour);

	return options;
};

export const getInitialFromTime = (
	areWorkingHours: boolean,
	initialDate?: Dayjs
) => {
	const nearestTime = getNearestQuarterHourTime(initialDate, undefined, true);
	const startOfDay = getFormattedTime(getDayStartHour(areWorkingHours), 0);

	const endOfDay = getFormattedTime(
		getDayEndHour(areWorkingHours) + (areWorkingHours ? 0 : 1),
		areWorkingHours ? MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES : 0
	);

	return nearestTime >= startOfDay && nearestTime <= endOfDay
		? nearestTime
		: startOfDay;
};

export const getInitialTime = (
	areWorkingHours: boolean,
	timezone: Timezone
): SelectedTime => {
	const currentOfficeTime = dayjs().tz(timezone, false);

	const initialFromTime = parseStringToDayjs(
		getInitialFromTime(areWorkingHours, currentOfficeTime)
	).tz(timezone, true);

	const initialToTime = parseStringToDayjs(
		getNearestQuarterHourTime(initialFromTime, 0, true)
	).tz(timezone, true);

	return {
		from: initialFromTime,
		to: initialToTime,
	};
};

export const getInitialTimelineDate = (timezone: Timezone) => {
	const officeCurrentDateTime = dayjs().tz(timezone, false);

	const nextTime = getInitialFromTime(true, officeCurrentDateTime);
	const currentTime = getNearestQuarterHourTime(
		officeCurrentDateTime,
		undefined,
		true
	);

	return nextTime < currentTime || currentTime === getFormattedTime(0, 0)
		? officeCurrentDateTime.add(1, 'day').startOf('day')
		: officeCurrentDateTime.startOf('day');
};

const mergeDateAndTime = (date: Dayjs, time: Dayjs) =>
	date.clone().set({
		hours: time.hour(),
		minutes: time.minute(),
		seconds: time.second(),
		milliseconds: time.millisecond(),
	});

export const isTimelineDateInPast = (timelineDate: Dayjs, timezone: Timezone) =>
	timelineDate
		.clone()
		.tz(timezone)
		.startOf('day')
		.isBefore(dayjs().tz(timezone).startOf('day'));

export const isMeetingRoomCanceliationDisabled = (
	meetingStart: Dayjs,
	meetingEnd: Dayjs,
	timezone: Timezone
) => {
	const now = dayjs().tz(timezone);

	if (meetingEnd.isBefore(now, 'day')) {
		return true;
	}

	if (meetingStart.isAfter(now, 'day')) {
		return false;
	}

	if (meetingEnd.isBefore(now)) {
		const diffInMinutes = now.diff(meetingEnd, 'minute');

		if (diffInMinutes <= 1) {
			return false;
		}

		return true;
	}

	return false;
};

export const getOfficeBookingRange = (timezone: Timezone) => {
	const currentOfficeDate = dayjs().tz(timezone);
	const currentOfficeDateInThreeMonth = currentOfficeDate
		.clone()
		.add({ months: AVAILABLE_PERIOD_TO_BOOK_IN_MONTH });

	return {
		currentOfficeDate,
		currentOfficeDateInThreeMonth,
	};
};

export const convertDayjsToDatePickerDate = (dateDayjs: Dayjs | null) => {
	if (dateDayjs) {
		const { months, years, date, hours, minutes } = dateDayjs.toObject();

		return new Date(years, months, date, hours, minutes);
	}

	return null;
};

export const convertDeskDateListToDayjs = (
	dateList: string[],
	timezone: Timezone
) => dateList.map((date) => dayjs(date).utc(false).tz(timezone, true));

export const isDateTodayWithTimezone = (date: Date, timezone: Timezone) => {
	const startOfCurrentDay = dayjs().tz(timezone).startOf('day');
	const startOfSelectedDay = dayjs(date).tz(timezone, true).startOf('day');

	return startOfCurrentDay.isSame(startOfSelectedDay, 'day');
};

const isZeroTime = (time: Dayjs) => time.hour() === 0 && time.minute() === 0;

const getConvertedBookingTime = (
	time: Dayjs,
	isAllDayChecked: boolean,
	zeroTimeAlternative: string
): Dayjs =>
	isAllDayChecked || isZeroTime(time)
		? dayjs(zeroTimeAlternative, RESPONSE_TIME_FORMAT)
		: time;

export const areMeetingRoomsOpen = (tabId: number | null) =>
	tabId === MEETING_ROOMS_TAB_ID;

export const getRequestBookingDateTime = (
	date: Dayjs,
	time: Dayjs,
	isAllDayChecked: boolean,
	zeroTimeAlternative: string,
	timezone: Timezone
) => {
	const convertedTime = getConvertedBookingTime(
		time,
		isAllDayChecked,
		zeroTimeAlternative
	);

	return parseDayjsToUtcString(mergeDateAndTime(date, convertedTime), timezone);
};

export const getRequestBookingDate = (date: Dayjs) =>
	date.format(YYYY_MM_DD_DAYJS_DATE_FORMAT);

export const normalizeCellIndex = (value: number) => {
	const mod = ((value % HOURS_IN_DAY) + HOURS_IN_DAY) % HOURS_IN_DAY;

	return mod;
};

export const isAlmostEqual = (a: number, b: number, eps = 1e-6) =>
	Math.abs(a - b) < eps;
