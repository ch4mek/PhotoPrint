import dayjs from 'dayjs';

import { TimelinePeriod } from '@/components/Timeline/constants';
import { Timezone } from '@/components/Timeline/types';
import { isMeetingRoomTimeInPast } from '@/helpers/isMeetingRoomTimeInPast';

describe('Helper function isMeetingRoomTimeInPast', () => {
	const timezone = Timezone.MINSK;
	const fixedNow = dayjs.tz('2025-01-01T10:00:00', timezone);

	beforeAll(() => {
		jest.useFakeTimers();
		jest.setSystemTime(fixedNow.toDate());
	});

	it('should return true for all-day bookings in the past', () => {
		const pastDate = fixedNow.subtract(1, 'day');
		expect(
			isMeetingRoomTimeInPast(
				pastDate,
				{ from: fixedNow, to: fixedNow },
				true,
				timezone
			)
		).toBe(true);
	});

	it('should return false for all-day bookings today', () => {
		expect(
			isMeetingRoomTimeInPast(
				fixedNow,
				{ from: fixedNow, to: fixedNow },
				true,
				timezone
			)
		).toBe(true);
	});

	it('should return true if time is before currentOfficeTime', () => {
		const pastTime = fixedNow.subtract(1, 'hour');
		expect(
			isMeetingRoomTimeInPast(
				fixedNow,
				{ from: pastTime, to: pastTime.add(30, 'minutes') },
				false,
				timezone
			)
		).toBe(true);
	});

	it('should return false if time is in the future', () => {
		const futureTime = fixedNow.add(1, 'hour');
		expect(
			isMeetingRoomTimeInPast(
				fixedNow,
				{ from: futureTime, to: futureTime.add(2, 'day') },
				false,
				timezone
			)
		).toBe(false);
	});

	it('should handle 15th and 45th minute rule correctly', () => {
		const fifteenMinutesPast = fixedNow
			.subtract(TimelinePeriod.QUARTER_PERIOD, 'hour')
			.minute(15);
		expect(
			isMeetingRoomTimeInPast(
				fixedNow,
				{ from: fifteenMinutesPast, to: fifteenMinutesPast.add(30, 'minutes') },
				false,
				timezone
			)
		).toBe(true);
	});
});
