import {
	BookedDataResponse,
	BookedDataTime,
	Section,
	SelectedTimePeriod,
} from '@/components/Timeline/types';
import {
	TIME_SELECTOR_STEP_IN_MINUTES,
	VIEW_TIME_FORMAT,
} from '@/constants/booking';
import { timeUnits } from '@/constants/date';
import {
	SelectedBookingItem,
	SelectedBookingItemType,
	SelectedRoom,
	SelectedTime,
} from '@/context/types';
import { getFormattedTime } from '@/helpers/bookingFormatters';
import { parseDateToTimeObject } from '@/helpers/bookingParsers';
import {
	getDayEndHour,
	getDayStartHour,
	getNearestQuarterHourTime,
	getTimeRange,
} from '@/helpers/bookingTime';
import dayjs, { Dayjs } from '@/lib/dayjs';
import {
	GetMeetingRoomsReservationsResponse,
	GetRoomsBookingResponse,
} from '@/queries/booking/types';
import { MeetingRoomBooking } from '@/types/booking';

const MINUTES_IN_HOUR = 60;

export const getSelectedPeriod = (
	selectedItem: SelectedBookingItem | null,
	selectedTime: SelectedTime | null,
	isTimelineInRange: boolean,
	isOverlapped: boolean,
	isAllDayChecked: boolean,
	isLoading: boolean
): SelectedTimePeriod<BookedDataTime> | null => {
	if (
		!isTimelineInRange ||
		selectedItem?.type === SelectedBookingItemType.Desk
	) {
		return null;
	}

	if (isAllDayChecked && selectedItem) {
		const toObject = {
			...dayjs().add(1, 'day').startOf('day').toObject(),
			hours: 24,
		};

		return {
			from: dayjs().startOf('day').toObject(),
			to: toObject,
			isOverlapped,
			isLoading,
		};
	}

	if (!selectedItem || !selectedTime) {
		return null;
	}

	const [from, to] = getTimeRange(selectedTime.from, selectedTime.to);

	const lastObject = to.toObject();
	const fromObject = from.toObject();

	return {
		from: fromObject,
		to:
			lastObject.hours === 0 && fromObject.hours !== 0
				? { ...lastObject, hours: timeUnits.HOURS_IN_DAY }
				: lastObject,
		isOverlapped,
		isLoading,
	};
};

const ZERO_TIME = getFormattedTime(0, 0);

export const getRelevantTimeForWorkingHours = (
	selectedTime: SelectedTime | null,
	areHoursWorking: boolean
): SelectedTime | null => {
	if (!selectedTime) {
		return null;
	}

	const startHour = getDayStartHour(areHoursWorking);
	const endHour = getDayEndHour(areHoursWorking);

	const startFromTime = getFormattedTime(startHour, 0);
	const endFromTime = getFormattedTime(
		endHour,
		MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES
	);

	const startToTime = getFormattedTime(
		startHour,
		TIME_SELECTOR_STEP_IN_MINUTES
	);
	const endToTime = getFormattedTime(endHour + 1, 0);

	const currentFromTime = getNearestQuarterHourTime(
		selectedTime.from,
		undefined
	);
	const currentToTime = getNearestQuarterHourTime(selectedTime.to, undefined);

	const newSelectedTime: { from?: Dayjs; to?: Dayjs } = {};

	if (currentFromTime === ZERO_TIME || currentFromTime < startFromTime) {
		newSelectedTime.from = dayjs(startFromTime, VIEW_TIME_FORMAT);
	}

	if (currentFromTime > endFromTime) {
		newSelectedTime.from = dayjs(endFromTime, VIEW_TIME_FORMAT);
	}

	if (currentToTime < startToTime) {
		newSelectedTime.to = dayjs(startToTime, VIEW_TIME_FORMAT);
	}

	if (currentToTime === ZERO_TIME || currentToTime > endToTime) {
		newSelectedTime.to = dayjs(endToTime, VIEW_TIME_FORMAT);
	}

	return Object.keys(newSelectedTime).length
		? { ...selectedTime, ...newSelectedTime }
		: null;
};

export const mapReservationsResponse = (
	response: GetMeetingRoomsReservationsResponse
): Array<BookedDataResponse> =>
	response.map(({ reservations, ...item }) => ({
		...item,
		reservations: reservations.map(
			({
				reservationFromOfficeDateTime,
				reservationToOfficeDateTime,
				isMeetingRoomEntryRepetitiveBooking,
				...entry
			}) => ({
				...entry,
				isRepetitive: isMeetingRoomEntryRepetitiveBooking,
				fromDateTime: parseDateToTimeObject(reservationFromOfficeDateTime),
				toDateTime: parseDateToTimeObject(reservationToOfficeDateTime),
			})
		),
	}));

export const convertSectionToRoom = (section: Section): SelectedRoom => ({
	type: SelectedBookingItemType.Room,
	id: section.meetingRoomId,
	name: section.meetingRoomName,
});

export const findMeetingRoomByEntryId = (
	rooms: GetRoomsBookingResponse | undefined,
	entryId: number
): MeetingRoomBooking | null => {
	if (!rooms) {
		return null;
	}

	const foundRoom = rooms.find((room) =>
		(room.reservations || []).some(({ id }) => id === entryId)
	);

	return foundRoom || null;
};
