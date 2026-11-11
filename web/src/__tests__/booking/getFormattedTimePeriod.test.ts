import { TFunction } from 'i18next';

import {
	RESPONSE_ALL_DAY_END_TIME_FORMAT,
	RESPONSE_ALL_DAY_START_TIME_FORMAT,
} from '@/constants/booking';
import { getFormattedTimePeriod } from '@/helpers/bookingFormatters';

const mockCommonT = ((key: string) => {
	if (key === 'allDay') {
		return 'All day';
	}

	if (key === 'time.hours') {
		return 'h';
	}

	if (key === 'time.minutes') {
		return 'min';
	}

	return key;
}) as TFunction<'common'>;

const testCasesWithWithoutDifference = [
	[
		`2024-09-30T${RESPONSE_ALL_DAY_START_TIME_FORMAT}`,
		`2024-09-30T${RESPONSE_ALL_DAY_END_TIME_FORMAT}`,
		'All day',
	],
	['2024-09-30T11:00:00', '2024-09-30T12:00:00', '11:00 - 12:00'],
	['2024-09-30T10:00:00', '2024-09-30T10:45:00', '10:00 - 10:45'],
	[
		'2024-09-30T23:45:00',
		`2024-09-30T${RESPONSE_ALL_DAY_END_TIME_FORMAT}`,
		'23:45 - 00:00',
	],
	['2024-09-30T12:45:00', '2024-09-30T14:00:00', '12:45 - 14:00'],
	['2024-09-30T12:45:00', '2024-09-30T14:15:00', '12:45 - 14:15'],
	['2024-09-30T00:00:00', '2024-09-30T23:45:00', '00:00 - 23:45'],
];

const testCasesWithTimeDifference = [
	[
		`2024-09-30T${RESPONSE_ALL_DAY_START_TIME_FORMAT}`,
		`2024-09-30T${RESPONSE_ALL_DAY_END_TIME_FORMAT}`,
		'All day',
	],
	['2024-09-30T11:00:00', '2024-09-30T12:00:00', '11:00 - 12:00 (1 h)'],
	['2024-09-30T10:00:00', '2024-09-30T10:45:00', '10:00 - 10:45 (45 min)'],
	[
		'2024-09-30T23:45:00',
		`2024-09-30T${RESPONSE_ALL_DAY_END_TIME_FORMAT}`,
		'23:45 - 00:00 (15 min)',
	],
	['2024-09-30T12:45:00', '2024-09-30T14:00:00', '12:45 - 14:00 (1 h 15 min)'],
	['2024-09-30T12:45:00', '2024-09-30T14:15:00', '12:45 - 14:15 (1 h 30 min)'],
	['2024-09-30T00:00:00', '2024-09-30T23:45:00', '00:00 - 23:45 (23 h 45 min)'],
];

describe('getFormattedTimePeriod helper test', () => {
	it.each(testCasesWithWithoutDifference)(
		'should return correct formatted time period',
		(fromTime, toTime, expectedResult) => {
			const calculateDifference = false;
			const result = getFormattedTimePeriod(
				fromTime,
				toTime,
				calculateDifference,
				mockCommonT
			);

			expect(result).toBe(expectedResult);
		}
	);

	it.each(testCasesWithTimeDifference)(
		'should return correct formatted time period with difference',
		(fromTime, toTime, expectedResult) => {
			const calculateDifference = true;
			const result = getFormattedTimePeriod(
				fromTime,
				toTime,
				calculateDifference,
				mockCommonT
			);

			expect(result).toBe(expectedResult);
		}
	);
});
