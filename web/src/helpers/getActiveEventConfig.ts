import { EVENTS_PARTICLES_CONFIG } from '@/constants/particleOverlayConfigs';
import dayjs from '@/lib/dayjs';
import { EventSchedule } from '@/types/events';

export function isEventActive(schedule: EventSchedule) {
	const now = dayjs();
	const start = dayjs(schedule.startDate);
	const end = dayjs(schedule.endDate);

	return now.isBetween(start, end);
}

export function getActiveEventConfig() {
	return EVENTS_PARTICLES_CONFIG.find((event) => isEventActive(event.schedule));
}
