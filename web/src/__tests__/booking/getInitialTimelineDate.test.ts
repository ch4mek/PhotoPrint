import {
	MINUTES_IN_HOUR,
	SECONDS_IN_MINUTE,
} from '@/components/Timeline/constants';
import { Timezone } from '@/components/Timeline/types';
import {
	TIME_SELECTOR_STEP_IN_MINUTES,
	WORKING_DAY_END_HOUR,
} from '@/constants/booking';
import { getInitialTimelineDate } from '@/helpers/bookingTime';
import dayjs from '@/lib/dayjs';

const minskTimezoneOffset = -180;
dayjs.tz.setDefault('UTC');

const padZero = (value: number) => String(value).padStart(2, '0');
const createISOTime = (day: number, hours: number, minutes: number) =>
	`2024-10-${padZero(day)}T${padZero(hours)}:${padZero(minutes)}:00.000Z`;

const getOfficeTimeTimestampInUTC = (
	isoDate: string,
	officeTimezoneOffset: number
) => Date.parse(isoDate) + officeTimezoneOffset * SECONDS_IN_MINUTE * 1000;

describe('getInitialTimelineDate helper tests', () => {
	beforeEach(() => {
		jest.useFakeTimers();
	});

	afterEach(() => {
		jest.clearAllMocks();
		jest.useRealTimers();
	});

	it('Should return correct initial date if time is before working day', () => {
		const hours = 0;
		const minutes = 0;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.get('dates')).toBe(day);
	});

	it('Should return correct initial date if time is the start of working day', () => {
		const hours = 8;
		const minutes = 0;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day);
	});

	it('Should return correct initial date if time is the middle of working day', () => {
		const hours = 13;
		const minutes = 0;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day);
	});

	it('Should return correct initial date if time is 19:30', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES * 2;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;

		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day);
	});

	it('Should return correct next day if time is 19:45', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day + 1);
	});

	it('Should return correct next day if time is 20:00', () => {
		const hours = 20;
		const minutes = 0;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day + 1);
	});

	it('Should return next day if time is 23:59', () => {
		const hours = 23;
		const minutes = 59;
		const day = 17;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(day, hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const initialDate = getInitialTimelineDate(timezone);

		expect(initialDate.date()).toBe(day + 1);
	});
});
