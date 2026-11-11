import { isEventActive } from '@/helpers/getActiveEventConfig';
import dayjs from '@/lib/dayjs';
import { EventSchedule } from '@/types/events';

describe('Helper function isEventActive', () => {
	it('Should return true when now is between start and end', () => {
		const eventSchedule: EventSchedule = {
			startDate: dayjs().subtract(1, 'day').toISOString(),
			endDate: dayjs().add(1, 'day').toISOString(),
		};

		expect(isEventActive(eventSchedule)).toBe(true);
	});

	it('Should return false when now is after end', () => {
		const eventSchedule: EventSchedule = {
			startDate: dayjs().subtract(2, 'day').toISOString(),
			endDate: dayjs().subtract(1, 'day').toISOString(),
		};

		expect(isEventActive(eventSchedule)).toBe(false);
	});

	it('Should return false when now is before start', () => {
		const eventSchedule: EventSchedule = {
			startDate: dayjs().add(1, 'day').toISOString(),
			endDate: dayjs().add(2, 'day').toISOString(),
		};

		expect(isEventActive(eventSchedule)).toBe(false);
	});
});
