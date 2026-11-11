import { Timezone } from '@/components/Timeline/types';
import { FULL_HOURS_IN_DAY, MAX_SECONDS_COUNT } from '@/constants/time';
import dayjs, { Dayjs } from '@/lib/dayjs';

export const parseTimeToTodayDate = (timeStr: string) => {
	const [hoursStr, minutesStr] = timeStr.split(':');
	const hours = parseInt(hoursStr, 10);
	const minutes = parseInt(minutesStr, 10);

	const now = dayjs();

	return now.hour(hours).minute(minutes).second(0).millisecond(0);
};

export const parseStringToDayjs = (timeStr: string) =>
	dayjs(parseTimeToTodayDate(timeStr), 'HH:mm');

export const parseDateToTimeObject = (date: string) => {
	const dateObject = dayjs(date).toObject();

	if (dateObject.seconds === MAX_SECONDS_COUNT) {
		dateObject.seconds = 0;
		dateObject.minutes = 0;
		dateObject.hours = FULL_HOURS_IN_DAY;
	}

	return dateObject;
};

export const parseDayjsToUtcString = (
	date: Dayjs,
	timezone: Timezone
): string => date.clone().tz(timezone, true).toISOString();
