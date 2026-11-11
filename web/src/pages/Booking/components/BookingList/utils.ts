import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { SelectedBookingItemType } from '@/context/types';
import { getDateFormat } from '@/helpers/getDateFormat';
import { isMeetingRoom } from '@/helpers/isMeetingRoom';
import dayjs, { Dayjs } from '@/lib/dayjs';
import {
	GetBookingResponse,
	GetRoomsBookingResponse,
	MeetingRoomReservationEntry,
} from '@/queries/booking/types';
import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

import {
	ConvertedDeleteDeskBookingItem,
	ConvertedDeleteMeetingRoomBookingItem,
	DeskBookingItems,
	RoomBookingEdgeDates,
	RoomBookingEdgeMilliseconds,
	RoomBookingItems,
} from './types';

export const getRepetitiveWeekDays = (
	dayjsDateList: Dayjs[],
	weeksInInterval: number
) => {
	const dateFrom = dayjsDateList[0];
	const dateTo = dayjsDateList[1];

	const repetitiveWeekDays: Dayjs[] = [];

	let middleDate = dayjs(dateFrom);

	while (middleDate.isSameOrBefore(dateTo)) {
		repetitiveWeekDays.push(middleDate.clone());
		middleDate = middleDate.add(weeksInInterval, 'week');
	}

	return repetitiveWeekDays;
};

export const getDateRange = (dateFrom: Dayjs, dateTo: Dayjs): Dayjs[] => {
	const dates: Dayjs[] = [];

	let currentDate = dateFrom.clone();

	while (currentDate.isSameOrBefore(dateTo, 'day')) {
		dates.push(currentDate.clone());
		currentDate = currentDate.add(1, 'day');
	}

	return dates;
};

export const getOrderedBookings = (
	deskBookingData: GetBookingResponse,
	roomsBookingData: GetRoomsBookingResponse,
	areMeetingRoomsOpen: boolean
): Array<RoomBookingItems | DeskBookingItems> => {
	if (areMeetingRoomsOpen) {
		return [
			{
				type: SelectedBookingItemType.Room,
				items: roomsBookingData,
			},
			{
				type: SelectedBookingItemType.Desk,
				items: deskBookingData.bookings,
			},
		];
	}

	return [
		{
			type: SelectedBookingItemType.Desk,
			items: deskBookingData.bookings,
		},
		{
			type: SelectedBookingItemType.Room,
			items: roomsBookingData,
		},
	];
};

export const convertBookingItemToPopupData = (
	item: MeetingRoomBooking | DeskBooking | null
):
	| null
	| ConvertedDeleteDeskBookingItem
	| ConvertedDeleteMeetingRoomBookingItem => {
	if (!item) {
		return null;
	}

	if (isMeetingRoom(item)) {
		return {
			type: SelectedBookingItemType.Room,
			isBlock: false,
			...item,
		};
	}

	return {
		type: SelectedBookingItemType.Desk,
		...item,
	};
};

export const getRoomBookingEdgeDates = (
	reservations: MeetingRoomReservationEntry[]
): RoomBookingEdgeDates => {
	const edgeMilliseconds: RoomBookingEdgeMilliseconds = reservations.reduce(
		(
			{ firstDate, lastDate },
			{ reservationFromOfficeDateTime, reservationToOfficeDateTime }
		) => ({
			firstDate: Math.min(
				firstDate,
				dayjs(reservationFromOfficeDateTime).unix() * 1000
			),
			lastDate: Math.max(
				lastDate,
				dayjs(reservationToOfficeDateTime).unix() * 1000
			),
		}),
		{ firstDate: Infinity, lastDate: 0 }
	);

	return {
		firstDate:
			getDateFormat(
				dayjs(edgeMilliseconds.firstDate),
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			) ?? '',
		lastDate:
			getDateFormat(
				dayjs(edgeMilliseconds.lastDate),
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			) ?? '',
	};
};
