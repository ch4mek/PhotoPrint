import {
	MINUTES_IN_HOUR,
	TimelinePeriod,
} from '@/components/Timeline/constants';
import { Timezone } from '@/components/Timeline/types';
import { SelectedTime } from '@/context/types';
import dayjs, { Dayjs } from '@/lib/dayjs';

const roundDownToNearestHalfHour = (dateTime: Dayjs): Dayjs =>
	dateTime.set({
		hour: dateTime.hour(),
		minute:
			dateTime.minute() -
			(dateTime.minute() % (MINUTES_IN_HOUR * TimelinePeriod.HALF_PERIOD)),
		second: 0,
		millisecond: 0,
	});

export const isMeetingRoomTimeInPast = (
	timelineDate: Dayjs,
	selectedTime: SelectedTime,
	isAllDayChecked: boolean,
	timezone: Timezone
) => {
	const timelineWithTimezoneTime = timelineDate.clone().tz(timezone, true);
	const currentTimeWithTimezone = dayjs().tz(timezone);

	if (isAllDayChecked) {
		return timelineWithTimezoneTime.isSameOrBefore(
			currentTimeWithTimezone,
			'day'
		);
	}

	const fromTimeWithTimezone = selectedTime.from.tz(timezone, true);
	const toTimeWithTimezone = selectedTime.to.tz(timezone, true);

	const fromDateTimeOnTimelineWithTimezone = timelineWithTimezoneTime.set({
		hour: fromTimeWithTimezone.hour(),
		minute: fromTimeWithTimezone.minute(),
		second: 0,
		millisecond: 0,
	});

	const toDateTimeOnTimelineWithTimezone = timelineWithTimezoneTime.set({
		hour: toTimeWithTimezone.hour(),
		minute: toTimeWithTimezone.minute(),
		second: 0,
		millisecond: 0,
	});

	const fromHalfHourSlotStart = roundDownToNearestHalfHour(
		fromDateTimeOnTimelineWithTimezone
	);
	const currentHalfHourSlotStart = roundDownToNearestHalfHour(
		currentTimeWithTimezone
	);

	return (
		toDateTimeOnTimelineWithTimezone.isBefore(
			fromDateTimeOnTimelineWithTimezone
		) ||
		toDateTimeOnTimelineWithTimezone.isSameOrBefore(currentTimeWithTimezone) ||
		fromHalfHourSlotStart.isBefore(currentHalfHourSlotStart)
	);
};
