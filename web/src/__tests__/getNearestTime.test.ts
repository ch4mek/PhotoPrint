import { getNearestQuarterHourTime } from '@/helpers/bookingTime';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { getNearestTimeSelectorOption } from '@/pages/Booking/components/BookingForm/components/TimeSelectors/utils';

type TestCase = [Dayjs, string];

const correctValuesWithSkipIfMultiple: Array<TestCase> = [
	[dayjs(new Date(2023, 1, 1, 10, 13)), '10:15'],
	[dayjs(new Date(2023, 1, 1, 10, 15)), '10:30'],
	[dayjs(new Date(2023, 1, 1, 17, 59)), '18:00'],
	[dayjs(new Date(2023, 1, 1, 13, 10)), '13:15'],
	[dayjs(new Date(2023, 1, 1, 12, 59)), '13:00'],
	[dayjs(new Date(2023, 1, 1, 14, 1)), '14:15'],
	[dayjs(new Date(2023, 1, 1, 15, 31)), '15:45'],
	[dayjs(new Date(2023, 1, 1, 23, 31)), '23:45'],
	[dayjs(new Date(2023, 1, 1, 23, 45)), '00:00'],
	[dayjs(new Date(2023, 1, 1, 23, 46)), '00:00'],
];

const correctValuesWIthNOSkipIfMultiple: Array<TestCase> = [
	[dayjs(new Date(2023, 1, 1, 8, 0)), '08:00'],
	[dayjs(new Date(2023, 1, 1, 10, 13)), '10:15'],
	[dayjs(new Date(2023, 1, 1, 10, 15)), '10:15'],
	[dayjs(new Date(2023, 1, 1, 13, 10)), '13:15'],
	[dayjs(new Date(2023, 1, 1, 12, 59)), '13:00'],
	[dayjs(new Date(2023, 1, 1, 14, 1)), '14:15'],
	[dayjs(new Date(2023, 1, 1, 13, 30)), '13:30'],
	[dayjs(new Date(2023, 1, 1, 15, 31)), '15:45'],
	[dayjs(new Date(2023, 1, 1, 23, 31)), '23:45'],
	[dayjs(new Date(2023, 1, 1, 23, 45)), '23:45'],
	[dayjs(new Date(2023, 1, 1, 23, 46)), '00:00'],
];

const correctValuesWithOffsetWithSkipIfMultiple: Array<TestCase> = [
	[dayjs(new Date(2023, 1, 1, 8, 0)), '08:30'],
	[dayjs(new Date(2023, 1, 1, 10, 15)), '10:45'],
	[dayjs(new Date(2023, 1, 1, 23, 45)), '00:15'],
	[dayjs(new Date(2023, 1, 1, 10, 30)), '11:00'],
	[dayjs(new Date(2023, 1, 1, 13, 10)), '13:30'],
	[dayjs(new Date(2023, 1, 1, 12, 59)), '13:15'],
	[dayjs(new Date(2023, 1, 1, 14, 1)), '14:30'],
	[dayjs(new Date(2023, 1, 1, 15, 31)), '16:00'],
	[dayjs(new Date(2023, 1, 1, 23, 31)), '00:00'],
	[dayjs(new Date(2023, 1, 1, 23, 45)), '00:15'],
];

const correctValuesWithOffsetWithNOSkipIfMultiple: Array<TestCase> = [
	[dayjs(new Date(2023, 1, 1, 8, 0)), '08:15'],
	[dayjs(new Date(2023, 1, 1, 10, 15)), '10:30'],
	[dayjs(new Date(2023, 1, 1, 13, 10)), '13:30'],
	[dayjs(new Date(2023, 1, 1, 12, 59)), '13:15'],
	[dayjs(new Date(2023, 1, 1, 14, 1)), '14:30'],
	[dayjs(new Date(2023, 1, 1, 15, 31)), '16:00'],
	[dayjs(new Date(2023, 1, 1, 15, 30)), '15:45'],
	[dayjs(new Date(2023, 1, 1, 23, 46)), '00:15'],
	[dayjs(new Date(2023, 1, 1, 23, 45)), '00:00'],
	[dayjs(new Date(2023, 1, 1, 23, 16)), '23:45'],
];

describe('getNearestQuarterHourTime helper tests', () => {
	it.each(correctValuesWithSkipIfMultiple)(
		'should return nearest quarter hour time',
		(date, expectedResult) => {
			const nearestTime = getNearestQuarterHourTime(date, undefined, true);
			expect(nearestTime).toBe(expectedResult);
		}
	);

	it.each(correctValuesWIthNOSkipIfMultiple)(
		'should return nearest quarter hour time if not multiple',
		(date, expectedResult) => {
			const nearestTime = getNearestQuarterHourTime(date, 0);
			expect(nearestTime).toBe(expectedResult);
		}
	);

	it.each(correctValuesWithOffsetWithSkipIfMultiple)(
		'should return nearest quarter hour time with offset',
		(date, expectedResult) => {
			const nearestTime = getNearestQuarterHourTime(date, 1, true);
			expect(nearestTime).toBe(expectedResult);
		}
	);

	it.each(correctValuesWithOffsetWithNOSkipIfMultiple)(
		'should return nearest quarter hour time with offset with if not multiple ',
		(date, expectedResult) => {
			const nearestTime = getNearestQuarterHourTime(date, 1);
			expect(nearestTime).toBe(expectedResult);
		}
	);

	it('should return nearest time with negative offset', () => {
		const nearestTime = getNearestQuarterHourTime(
			dayjs(new Date(2024, 9, 6, 10, 1)),
			-1
		);
		expect(nearestTime).toBe('10:00');
	});
});

describe('getNearestTimeSelectorOption helper tests', () => {
	it.each(correctValuesWIthNOSkipIfMultiple)(
		'should return nearest time selector option',
		(date, expectedResult) => {
			const nearestTimeOption = getNearestTimeSelectorOption(date);
			expect(nearestTimeOption).toEqual({
				id: expectedResult,
				name: expectedResult,
			});
		}
	);

	it.each(correctValuesWithOffsetWithNOSkipIfMultiple)(
		'should return nearest time selector option with offset',
		(date, expectedResult) => {
			const nearestTimeOption = getNearestTimeSelectorOption(date, 1);
			expect(nearestTimeOption).toEqual({
				id: expectedResult,
				name: expectedResult,
			});
		}
	);
});
